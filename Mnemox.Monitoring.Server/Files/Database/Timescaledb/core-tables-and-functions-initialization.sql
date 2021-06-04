create table if not exists server.database_states(
	database_state_id bigserial,
	state_id smallint,
	insert_date_time_utc timestamp without time zone null default timezone('utc'::text, now()),
	constraint database_state_pk primary key (database_state_id)
);

create index on server.database_states("insert_date_time_utc");


create table if not exists tenants.tenants (
	tenant_id bigserial,
	tenant_name character varying(255) not null,
	tenant_description character varying(500) null,
	insert_date_time_utc timestamp(0) null default timezone('utc'::text, now()),
	constraint tokens_pk primary key (tenant_id)
);

create table if not exists tenants.tenants_objects (
	tenant_id bigint,
	object_id bigint,
	object_type_id smallint,
	constraint tenants_objects_pk primary key (tenant_id)
);

create table if not exists tenants.users (
	user_id bigserial,
	username character varying(255) not null,
	password character varying(255) not null,
	email character varying(255) null,
	first_name character varying(255) null,
	last_name character varying(255) null,
	insert_date_time_utc timestamp(0) null default timezone('utc'::text, now()),
	constraint users_pk primary key (user_id)
);


create table if not exists tenants.tokens (
	token_id bigserial not null,
	"token" character varying(500) not null,
	owner_id bigint not null,
	owner_type_id integer not null, -- 1 user, 2 resource
	valid_until_utc timestamp without time zone,
	constraint tokens_pk primary key (token_id)
);
create index on tenants.tokens("token");


create table if not exists resources.resources (
	resource_id bigserial,
	resource_name character varying(255) not null,
	resource_description character varying(500) null,
	resource_type smallint not null,
	insert_date_time_utc timestamp(0) NULL DEFAULT timezone('utc'::text, now())
);


create or replace function monitoring.heart_beats_add(p_instance_id bigint, p_tenant_id bigint)
 returns void
 language plpgsql
as $function$
begin

	insert into monitoring.heart_beats(instance_id, tenant_id)
	values(p_instance_id, p_tenant_id);

end;
$function$;

DROP FUNCTION resources.resources_add(character varying,character varying,smallint,bigint) ;
create or replace function resources.resources_add(
	p_resource_name character varying(255),
	p_resource_description character varying(500),
	p_resource_type smallint,
	p_tenant_id bigint)
 returns bigint
 language plpgsql
as $function$
	declare o_resource_id bigint;
begin

	insert into resources.resources(resource_name, resource_description, resource_type, tenant_id)
	values(p_resource_name, p_resource_description, p_resource_type)
	returning resource_id into o_resource_id;

	return o_resource_id;
end;
$function$;

create or replace function tenants.users_add(
	p_username character varying(255),
	p_password character varying(255),
	p_email character varying(255),
	p_first_name character varying(255),
	p_last_name character varying(255))
 returns bigint
 language plpgsql
as $function$
	declare o_user_id bigint;
begin

	insert into tenants.users(username, password, email, first_name, last_name)
	values(p_username, tenants.crypt(p_password, tenants.gen_salt('bf', 8)), p_email, p_first_name, p_last_name)
	returning user_id into o_user_id;

	return o_user_id;

end;
$function$;

create or replace function tenants.users_authenticate(
	p_username character varying(255),
	p_password character varying(255))
 returns table(o_user_id bigint, o_email character varying(255), o_first_name character varying(255), o_last_name character varying(255))
 language plpgsql
as $function$
begin

	return query
	select user_id, email, first_name, last_name from tenants.users where username = p_username and "password" = tenants.crypt(p_password, "password");

end;
$function$;


drop function if exists tenants.tokens_add(character varying, bigint, integer, timestamp without time zone) ;
create or replace function tenants.tokens_add(
	p_token character varying(500), 
	p_owner_id bigint, 
	p_owner_type_id integer, 
	p_valid_until_utc timestamp without time zone)
 returns bigint
 language plpgsql
as $function$
	declare o_token_id bigint;
begin

	insert into tenants.tokens("token", owner_id, owner_type_id, valid_until_utc)
	values(p_token, p_owner_id, p_owner_type_id, p_valid_until_utc)
	returning token_id into o_token_id;

	return o_token_id;
end;
$function$;


drop function if exists tenants.tokens_get_details(character varying);
create or replace function tenants.tokens_get_details(p_token character varying(500))
 returns table(
 		o_token_id bigint, 
 		o_token character varying(500), 
 		o_owner_id bigint,
 		o_owner_type_id integer,
 		o_valid_until_utc timestamp without time zone)
 language plpgsql
as $function$
begin

	return query
	select token_id, "token", owner_id, owner_type_id, valid_until_utc 
	from tenants.tokens where "token" = p_token and valid_until_utc > now() at time zone 'utc';

end;
$function$;


drop function if exists tenants.tenants_objects_add(bigint, bigint, smallint);
create or replace function tenants.tenants_objects_add(
	p_tenant_id bigint, 
	p_object_id bigint,
	p_object_type_id smallint)
 returns void
 language plpgsql
as $function$
begin

	insert into tenants.tenants_objects (tenant_id, object_id, object_type_id)
	values(p_tenant_id, p_object_id, p_object_type_id);

end;
$function$;


drop function if exists tenants.tenants_objects_get_by_object(bigint, smallint);
create or replace function tenants.tenants_objects_get_by_object(p_object_id bigint, p_object_type_id smallint)
 returns table(o_tenant_id bigint, o_tenant_name character varying, o_tenant_description character varying(500))
 language plpgsql
as $function$
begin

	return query
	select t.tenant_id, t.tenant_name, t.tenant_description
	from tenants.tenants t inner join tenants.tenants_objects tno on
	t.tenant_id = tno.tenant_id where tno.object_id = p_object_id and tno.object_type_id = p_object_type_id;

end;
$function$;

drop function if exists server.database_states_get_last();
create or replace function server.database_states_get_last()
 returns smallint
 language plpgsql
as $function$
declare r_state_id smallint;
begin

	select state_id into r_state_id from server.database_states order by insert_date_time_utc desc limit 1;

	return r_state_id;

end;
$function$;