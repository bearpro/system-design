create table subject (
    id int auto_increment primary key,
    name text not null,
    short_name text not null
);

create table study_group (
    id int auto_increment primary key,
    name text not null
);

create table student (
    id int auto_increment primary key,
    surname text not null,
    name text not null,
    second_name text not null,
    study_group_id int not null,
    foreign key (study_group_id) references study_group(id)
);

create table exam_type (
    id int auto_increment primary key,
    type text not null
);

create table study_plan (
    id int auto_increment primary key,
    subject_id int not null,
    exam_type_id int not null,
    foreign key (subject_id) references subject(id),
    foreign key (exam_type_id) references exam_type(id)
);

create table mark (
    id int auto_increment primary key,
    name text not null,
    value text not null
);

create table journal (
    id int auto_increment primary key,
    student_id int not null,
    study_plan_id int not null,
    in_time bit not null,
    `count` int not null,
    mark_id int not null,
    foreign key (student_id) references student(id),
    foreign key (study_plan_id) references study_plan(id)
    
);

insert into subject values 
    (1, 'Проектирование информационных систем', 'ПриС'),
    (2, 'Системы искусственного интеллекта', 'Сии'),
    (3, 'Программная инженерия', 'Пи'),
    (4, 'Национальная система информационной безопасности', 'НСиБ'),
    (5, 'Системный анализ', 'СисАнал'),
    (6, 'Распределенные базы данных', 'РБД'),
    (7, 'Системное программное обеспечение', 'СПО');

insert into exam_type values
    (1, 'Экзамен'),
    (2, 'Зачет'),
    (3, 'Зачет с оценкой'),
    (4, 'Курсовая');

insert into study_plan values 
    (1, 1, 1),
    (2, 1, 4),
    (3, 2, 1),
    (4, 3, 1),
    (5, 4, 2),
    (6, 5, 1),
    (7, 6, 2),
    (8, 7, 1);

insert into mark values 
    (1, 'Отлично', 5),
    (2, 'Хорошо', 4),
    (3, 'Удовлетворительно', 3),
    (4, 'Неудовлетворительно', 2),
    (5, 'Зачет', 'з'),
    (6, 'Незачет', 'н'),
    (7, 'Неявка', '');