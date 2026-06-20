-- phpMyAdmin SQL Dump
-- version 4.8.5
-- https://www.phpmyadmin.net/
--
-- Хост: localhost
-- Время создания: Июн 20 2026 г., 17:00
-- Версия сервера: 5.7.25
-- Версия PHP: 7.1.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `classroom_management`
--

-- --------------------------------------------------------

--
-- Структура таблицы `audit_logs`
--

CREATE TABLE `audit_logs` (
  `id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `action` varchar(255) CHARACTER SET utf8 NOT NULL,
  `details` text COLLATE utf8mb4_unicode_ci,
  `ip_address` varchar(45) CHARACTER SET utf8 DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `audit_logs`
--

INSERT INTO `audit_logs` (`id`, `user_id`, `action`, `details`, `ip_address`, `created_at`) VALUES
(1, 1, 'Login', 'Пользователь admin вошёл в систему', '127.0.0.1', '2026-06-19 21:19:21'),
(2, 1, 'Login', 'Пользователь admin вошёл в систему', '127.0.0.1', '2026-06-19 21:23:33'),
(3, 1, 'Login', 'Пользователь admin вошёл в систему', '127.0.0.1', '2026-06-19 21:34:05'),
(4, 1, 'Login', 'Пользователь admin вошёл в систему', '127.0.0.1', '2026-06-19 21:40:06'),
(5, 1, 'Login', 'Пользователь admin вошёл в систему', '127.0.0.1', '2026-06-20 19:52:37'),
(6, 1, 'Login', 'Пользователь admin вошёл в систему', '127.0.0.1', '2026-06-20 19:53:34'),
(7, 1, 'Login', 'Пользователь admin вошёл в систему', '127.0.0.1', '2026-06-20 19:58:09'),
(8, 1, 'Login', 'Пользователь admin вошёл в систему', '127.0.0.1', '2026-06-20 19:59:44');

-- --------------------------------------------------------

--
-- Структура таблицы `bookings`
--

CREATE TABLE `bookings` (
  `id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `classroom_id` int(11) DEFAULT NULL,
  `requested_room_type_id` int(11) DEFAULT NULL,
  `requested_capacity` int(11) DEFAULT NULL,
  `preferred_building_id` int(11) DEFAULT NULL,
  `preferred_equipment` text COLLATE utf8mb4_unicode_ci,
  `event_date` date NOT NULL,
  `start_time` time NOT NULL,
  `end_time` time NOT NULL,
  `pair_number` int(11) DEFAULT NULL COMMENT '1-5 номер пары',
  `purpose` varchar(500) CHARACTER SET utf8 DEFAULT NULL,
  `periodicity` varchar(50) CHARACTER SET utf8 DEFAULT 'once',
  `period_end_date` date DEFAULT NULL,
  `status_id` int(11) NOT NULL,
  `approved_by` int(11) DEFAULT NULL,
  `approved_room_id` int(11) DEFAULT NULL,
  `comment` varchar(500) CHARACTER SET utf8 DEFAULT NULL,
  `is_urgent` bit(1) NOT NULL DEFAULT b'0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `bookings`
--

INSERT INTO `bookings` (`id`, `teacher_id`, `classroom_id`, `requested_room_type_id`, `requested_capacity`, `preferred_building_id`, `preferred_equipment`, `event_date`, `start_time`, `end_time`, `pair_number`, `purpose`, `periodicity`, `period_end_date`, `status_id`, `approved_by`, `approved_room_id`, `comment`, `is_urgent`, `created_at`, `updated_at`) VALUES
(1, 1, 2, NULL, NULL, NULL, NULL, '2026-06-18', '08:00:00', '09:35:00', 1, 'Лекция по высшей математике', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(2, 2, 5, NULL, NULL, NULL, NULL, '2026-06-19', '11:30:00', '13:05:00', 3, 'Практика по физике', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(3, 3, 8, NULL, NULL, NULL, NULL, '2026-06-20', '09:45:00', '11:20:00', 2, 'Семинар по философии', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(4, 4, 11, NULL, NULL, NULL, NULL, '2026-06-21', '13:35:00', '15:10:00', 4, 'Лабораторная по химии', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(5, 5, 14, NULL, NULL, NULL, NULL, '2026-06-22', '15:20:00', '16:55:00', 5, 'Консультация по программированию', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(6, 6, 17, NULL, NULL, NULL, NULL, '2026-06-23', '08:00:00', '09:35:00', 1, 'Лекция по экономике', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(7, 7, 20, NULL, NULL, NULL, NULL, '2026-06-24', '11:30:00', '13:05:00', 3, 'Практика по менеджменту', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(8, 8, 22, NULL, NULL, NULL, NULL, '2026-06-25', '09:45:00', '11:20:00', 2, 'Семинар по маркетингу', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(9, 9, 1, NULL, NULL, NULL, NULL, '2026-06-26', '13:35:00', '15:10:00', 4, 'Лекция по статистике', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(10, 10, 3, NULL, NULL, NULL, NULL, '2026-06-27', '15:20:00', '16:55:00', 5, 'Лабораторная по информатике', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(11, 11, 6, NULL, NULL, NULL, NULL, '2026-06-28', '08:00:00', '09:35:00', 1, 'Практика по английскому', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(12, 12, 9, NULL, NULL, NULL, NULL, '2026-06-29', '11:30:00', '13:05:00', 3, 'Лекция по истории', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(13, 1, 10, NULL, NULL, NULL, NULL, '2026-06-18', '09:45:00', '11:20:00', 2, 'Лекция по математике (утв)', 'once', NULL, 2, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(14, 2, 13, NULL, NULL, NULL, NULL, '2026-06-19', '13:35:00', '15:10:00', 4, 'Практика по физике (утв)', 'once', NULL, 2, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(15, 3, 16, NULL, NULL, NULL, NULL, '2026-06-20', '15:20:00', '16:55:00', 5, 'Семинар по философии (утв)', 'once', NULL, 2, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(16, 4, 19, NULL, NULL, NULL, NULL, '2026-06-21', '08:00:00', '09:35:00', 1, 'Лабораторная по химии (утв)', 'once', NULL, 2, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(17, 5, 21, NULL, NULL, NULL, NULL, '2026-06-22', '11:30:00', '13:05:00', 3, 'Консультация по программированию (утв)', 'once', NULL, 2, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(18, 6, 23, NULL, NULL, NULL, NULL, '2026-06-23', '09:45:00', '11:20:00', 2, 'Лекция по экономике (утв)', 'once', NULL, 2, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(19, 7, 25, NULL, NULL, NULL, NULL, '2026-06-24', '13:35:00', '15:10:00', 4, 'Практика по менеджменту (утв)', 'once', NULL, 2, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(20, 8, 2, NULL, NULL, NULL, NULL, '2026-06-25', '15:20:00', '16:55:00', 5, 'Семинар по маркетингу (утв)', 'once', NULL, 2, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(21, 9, 4, NULL, NULL, NULL, NULL, '2026-06-26', '08:00:00', '09:35:00', 1, 'Лекция по статистике (утв)', 'once', NULL, 2, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(22, 10, 7, NULL, NULL, NULL, NULL, '2026-06-27', '11:30:00', '13:05:00', 3, 'Лабораторная по информатике (утв)', 'once', NULL, 2, NULL, NULL, NULL, b'0', '2026-06-19 21:18:49', '2026-06-19 21:18:49'),
(23, 11, 12, NULL, NULL, NULL, NULL, '2026-06-15', '08:00:00', '09:35:00', 1, 'Лекция по биологии', 'once', NULL, 3, NULL, NULL, NULL, b'0', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(24, 12, 15, NULL, NULL, NULL, NULL, '2026-06-16', '11:30:00', '13:05:00', 3, 'Практика по географии', 'once', NULL, 3, NULL, NULL, NULL, b'0', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(25, 13, 18, NULL, NULL, NULL, NULL, '2026-06-17', '09:45:00', '11:20:00', 2, 'Семинар по экологии', 'once', NULL, 3, NULL, NULL, NULL, b'0', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(26, 14, 24, NULL, NULL, NULL, NULL, '2026-06-18', '13:35:00', '15:10:00', 4, 'Лабораторная по астрономии', 'once', NULL, 3, NULL, NULL, NULL, b'0', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(27, 15, 5, NULL, NULL, NULL, NULL, '2026-06-19', '15:20:00', '16:55:00', 5, 'Консультация по социологии', 'once', NULL, 3, NULL, NULL, NULL, b'0', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(28, 1, 3, NULL, NULL, NULL, NULL, '2026-06-19', '08:00:00', '09:35:00', 1, 'Срочная консультация по математике', 'once', NULL, 1, NULL, NULL, NULL, b'1', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(29, 2, 6, NULL, NULL, NULL, NULL, '2026-06-19', '11:30:00', '13:05:00', 3, 'Срочная практика по физике', 'once', NULL, 1, NULL, NULL, NULL, b'1', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(30, 3, 9, NULL, NULL, NULL, NULL, '2026-06-20', '09:45:00', '11:20:00', 2, 'Срочный семинар по философии', 'once', NULL, 1, NULL, NULL, NULL, b'1', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(31, 4, 14, NULL, NULL, NULL, NULL, '2026-06-20', '13:35:00', '15:10:00', 4, 'Срочная лабораторная по химии', 'once', NULL, 1, NULL, NULL, NULL, b'1', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(32, 5, 17, NULL, NULL, NULL, NULL, '2026-06-22', '08:00:00', '09:35:00', 1, 'Еженедельная лекция по программированию', 'weekly', '2026-07-20', 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(33, 6, 20, NULL, NULL, NULL, NULL, '2026-06-23', '11:30:00', '13:05:00', 3, 'Еженедельная практика по экономике', 'weekly', '2026-07-21', 1, NULL, NULL, NULL, b'0', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(34, 7, 22, NULL, NULL, NULL, NULL, '2026-06-24', '09:45:00', '11:20:00', 2, 'Еженедельный семинар по менеджменту', 'weekly', '2026-07-22', 2, NULL, NULL, NULL, b'0', '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(100, 100, 100, NULL, NULL, NULL, NULL, '2026-08-01', '09:45:00', '11:20:00', 2, 'Лекция по экологии', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:23:06', '2026-06-19 21:23:06'),
(101, 101, 102, NULL, NULL, NULL, NULL, '2026-08-02', '13:35:00', '15:10:00', 4, 'Практика по робототехнике', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:23:06', '2026-06-19 21:23:06'),
(102, 102, 103, NULL, NULL, NULL, NULL, '2026-08-03', '08:00:00', '09:35:00', 1, 'Лекция по психологии', 'once', NULL, 2, 1, 103, 'Утверждено', b'0', '2026-06-19 21:23:06', '2026-06-19 21:23:06'),
(103, 103, 101, NULL, NULL, NULL, NULL, '2026-08-04', '11:30:00', '13:05:00', 3, 'Лабораторная работа по физике', 'once', NULL, 3, 1, NULL, 'Отклонено: нет оборудования', b'0', '2026-06-19 21:23:06', '2026-06-19 21:23:06'),
(104, 100, 100, NULL, NULL, NULL, NULL, '2026-08-05', '15:20:00', '16:55:00', 5, 'Консультация перед экзаменом (срочно)', 'once', NULL, 1, NULL, NULL, NULL, b'1', '2026-06-19 21:23:06', '2026-06-19 21:23:06'),
(105, 101, 102, NULL, NULL, NULL, NULL, '2026-08-06', '09:45:00', '11:20:00', 2, 'Робототехника (еженедельно)', 'weekly', '2026-09-03', 2, 1, 102, NULL, b'0', '2026-06-19 21:23:06', '2026-06-19 21:23:06'),
(106, 103, 101, NULL, NULL, NULL, NULL, '2026-08-07', '13:35:00', '15:10:00', 4, 'Практические занятия (ежедневно)', 'daily', '2026-08-13', 1, NULL, NULL, NULL, b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(107, 100, 100, NULL, NULL, NULL, NULL, '2026-08-08', '08:00:00', '09:35:00', 1, 'Лекция по биологии', 'once', NULL, 2, 1, 100, NULL, b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(108, 102, 103, NULL, NULL, NULL, NULL, '2026-08-09', '11:30:00', '13:05:00', 3, 'Срочное собрание сотрудников', 'once', NULL, 1, NULL, NULL, NULL, b'1', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(109, 101, 102, NULL, NULL, NULL, NULL, '2026-08-10', '15:20:00', '16:55:00', 5, 'Дополнительное занятие', 'once', NULL, 4, NULL, NULL, 'Отменено преподавателем', b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(110, 103, 101, NULL, NULL, NULL, NULL, '2026-08-11', '09:45:00', '11:20:00', 2, 'Лабораторная работа по химии', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(111, 100, 100, NULL, NULL, NULL, NULL, '2026-08-12', '13:35:00', '15:10:00', 4, 'Семинар по экологии', 'once', NULL, 2, 1, 100, NULL, b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(112, 102, 103, NULL, NULL, NULL, NULL, '2026-08-13', '08:00:00', '09:35:00', 1, 'Лекция по философии', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(113, 101, 102, NULL, NULL, NULL, NULL, '2026-08-14', '11:30:00', '13:05:00', 3, 'Робототехника (второй поток)', 'weekly', '2026-09-18', 1, NULL, NULL, NULL, b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(114, 103, 101, NULL, NULL, NULL, NULL, '2026-08-15', '15:20:00', '16:55:00', 5, 'Практика по физике', 'once', NULL, 2, 1, 101, NULL, b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(115, 100, 100, NULL, NULL, NULL, NULL, '2026-08-20', '08:00:00', '09:35:00', 1, 'Консультация по экологии', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(116, 101, 102, NULL, NULL, NULL, NULL, '2026-08-21', '11:30:00', '13:05:00', 3, 'Лекция по робототехнике', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(117, 102, 103, NULL, NULL, NULL, NULL, '2026-08-22', '13:35:00', '15:10:00', 4, 'Семинар по психологии', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(118, 103, 101, NULL, NULL, NULL, NULL, '2026-08-23', '15:20:00', '16:55:00', 5, 'Лабораторная по физике (группа 3)', 'once', NULL, 1, NULL, NULL, NULL, b'0', '2026-06-19 21:23:07', '2026-06-19 21:23:07');

-- --------------------------------------------------------

--
-- Структура таблицы `booking_statuses`
--

CREATE TABLE `booking_statuses` (
  `id` int(11) NOT NULL,
  `name` varchar(50) CHARACTER SET utf8 NOT NULL,
  `description` varchar(255) CHARACTER SET utf8 DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `booking_statuses`
--

INSERT INTO `booking_statuses` (`id`, `name`, `description`) VALUES
(1, 'Pending', 'На рассмотрении'),
(2, 'Approved', 'Утверждена'),
(3, 'Rejected', 'Отклонена'),
(4, 'Cancelled', 'Отменена');

-- --------------------------------------------------------

--
-- Структура таблицы `buildings`
--

CREATE TABLE `buildings` (
  `id` int(11) NOT NULL,
  `name` varchar(100) CHARACTER SET utf8 NOT NULL,
  `address` varchar(255) CHARACTER SET utf8 DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `buildings`
--

INSERT INTO `buildings` (`id`, `name`, `address`) VALUES
(1, 'Основной корпус', 'ул. Примерная, д. 1'),
(2, 'Учебный корпус №2', 'ул. Примерная, д. 2'),
(3, 'Корпус №3', 'ул. Новая, д. 5'),
(4, 'Спортивный комплекс', 'ул. Спортивная, 10');

-- --------------------------------------------------------

--
-- Структура таблицы `classrooms`
--

CREATE TABLE `classrooms` (
  `id` int(11) NOT NULL,
  `number` varchar(20) CHARACTER SET utf8 NOT NULL,
  `building_id` int(11) NOT NULL,
  `floor` int(11) NOT NULL,
  `room_type_id` int(11) NOT NULL,
  `capacity` int(11) NOT NULL,
  `condition_id` int(11) NOT NULL,
  `long_term_booking_by` varchar(200) CHARACTER SET utf8 DEFAULT NULL,
  `long_term_until` datetime DEFAULT NULL,
  `equipment_note` text COLLATE utf8mb4_unicode_ci,
  `department_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `classrooms`
--

INSERT INTO `classrooms` (`id`, `number`, `building_id`, `floor`, `room_type_id`, `capacity`, `condition_id`, `long_term_booking_by`, `long_term_until`, `equipment_note`, `department_id`) VALUES
(1, '101', 1, 1, 1, 30, 1, 'Иванов Иван Иванович', '2026-12-31 23:59:59', NULL, 1),
(2, '102', 1, 1, 1, 25, 1, NULL, NULL, NULL, 1),
(3, '103', 1, 1, 3, 20, 1, NULL, NULL, NULL, 1),
(4, '104', 1, 1, 1, 30, 1, 'Петрова Мария Сергеевна', '2026-06-30 23:59:59', NULL, 2),
(5, '105', 1, 1, 1, 25, 1, NULL, NULL, NULL, 2),
(6, '106', 1, 1, 2, 15, 2, NULL, NULL, NULL, 2),
(7, '107', 1, 1, 3, 20, 1, 'Сидоров Алексей Петрович', '2026-08-15 23:59:59', NULL, 3),
(8, '108', 1, 1, 1, 30, 1, NULL, NULL, NULL, 3),
(9, '201', 1, 2, 1, 30, 3, NULL, NULL, NULL, 2),
(10, '202', 1, 2, 1, 30, 1, 'Козлова Елена Викторовна', '2026-09-01 23:59:59', NULL, 3),
(11, '203', 1, 2, 4, 100, 1, NULL, NULL, NULL, 3),
(12, '204', 1, 2, 2, 15, 2, NULL, NULL, NULL, 4),
(13, '205', 1, 2, 1, 25, 1, NULL, NULL, NULL, 4),
(14, '206', 1, 2, 3, 20, 1, NULL, NULL, NULL, 5),
(15, '207', 1, 2, 1, 30, 1, 'Михайлов Дмитрий Сергеевич', '2026-07-20 23:59:59', NULL, 5),
(16, '301', 2, 1, 1, 30, 1, NULL, NULL, NULL, 1),
(17, '302', 2, 1, 1, 25, 1, NULL, NULL, NULL, 2),
(18, '303', 2, 1, 3, 20, 2, NULL, NULL, NULL, 2),
(19, '304', 2, 1, 2, 15, 1, NULL, NULL, NULL, 3),
(20, '305', 2, 1, 1, 30, 1, NULL, NULL, NULL, 4),
(21, '401', 2, 2, 1, 30, 1, NULL, NULL, NULL, 4),
(22, '402', 2, 2, 4, 120, 1, NULL, NULL, NULL, 5),
(23, '403', 2, 2, 3, 20, 1, NULL, NULL, NULL, 5),
(24, '501', 3, 1, 1, 30, 3, NULL, NULL, NULL, 1),
(25, '502', 3, 1, 1, 25, 1, NULL, NULL, NULL, 2),
(100, '501', 2, 5, 1, 40, 1, 'Новикова Ольга Петровна', '2026-12-31 23:59:59', NULL, 2),
(101, '502', 2, 5, 2, 25, 1, NULL, NULL, NULL, 2),
(102, '503', 2, 5, 3, 22, 1, 'Зайцев Андрей Владимирович', '2026-09-30 23:59:59', NULL, 1),
(103, '504', 2, 5, 4, 150, 1, NULL, NULL, NULL, 3);

-- --------------------------------------------------------

--
-- Структура таблицы `classroom_equipment`
--

CREATE TABLE `classroom_equipment` (
  `classroom_id` int(11) NOT NULL,
  `equipment_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `classroom_equipment`
--

INSERT INTO `classroom_equipment` (`classroom_id`, `equipment_id`) VALUES
(1, 1),
(2, 1),
(4, 1),
(5, 1),
(7, 1),
(8, 1),
(9, 1),
(10, 1),
(11, 1),
(13, 1),
(15, 1),
(16, 1),
(17, 1),
(20, 1),
(21, 1),
(22, 1),
(24, 1),
(25, 1),
(100, 1),
(103, 1),
(1, 2),
(4, 2),
(7, 2),
(9, 2),
(13, 2),
(15, 2),
(16, 2),
(20, 2),
(21, 2),
(24, 2),
(100, 2),
(1, 3),
(2, 3),
(3, 3),
(6, 3),
(9, 3),
(12, 3),
(14, 3),
(15, 3),
(17, 3),
(18, 3),
(19, 3),
(21, 3),
(23, 3),
(24, 3),
(101, 3),
(102, 3),
(3, 4),
(6, 4),
(8, 4),
(11, 4),
(12, 4),
(14, 4),
(18, 4),
(19, 4),
(22, 4),
(23, 4),
(101, 4),
(3, 5),
(17, 5),
(23, 5),
(102, 5),
(5, 6),
(10, 6),
(20, 6),
(25, 6),
(6, 7),
(8, 7),
(11, 7),
(19, 7),
(22, 7),
(25, 7),
(103, 7),
(7, 8),
(103, 8),
(11, 12),
(22, 12),
(12, 13),
(13, 14),
(14, 15),
(18, 16),
(100, 100),
(101, 101),
(102, 102),
(102, 103),
(103, 104);

-- --------------------------------------------------------

--
-- Структура таблицы `departments`
--

CREATE TABLE `departments` (
  `id` int(11) NOT NULL,
  `name` varchar(200) CHARACTER SET utf8 NOT NULL,
  `abbreviation` varchar(20) CHARACTER SET utf8 NOT NULL,
  `head_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `departments`
--

INSERT INTO `departments` (`id`, `name`, `abbreviation`, `head_id`) VALUES
(1, 'Заочное отделение', 'ЗО', NULL),
(2, 'Дневное отделение', 'ДО', NULL),
(3, 'Вечернее отделение', 'ВО', NULL),
(4, 'Магистратура', 'Маг', NULL),
(5, 'Аспирантура', 'Асп', NULL);

-- --------------------------------------------------------

--
-- Структура таблицы `equipment`
--

CREATE TABLE `equipment` (
  `id` int(11) NOT NULL,
  `name` varchar(100) CHARACTER SET utf8 NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `equipment`
--

INSERT INTO `equipment` (`id`, `name`) VALUES
(1, 'Проектор'),
(2, 'Интерактивная доска'),
(3, 'Компьютер'),
(4, 'Ноутбук'),
(5, 'Принтер'),
(6, 'Документ-камера'),
(7, 'Акустическая система'),
(8, 'Микрофон'),
(9, 'Телевизор'),
(10, 'Wi-Fi роутер'),
(11, 'Видеокамера'),
(12, 'Экран'),
(13, 'Маркерная доска'),
(14, 'Магнитно-маркерная доска'),
(15, 'Мультимедиа-проектор'),
(16, 'Доска для мела'),
(17, 'Спиннер'),
(18, 'Колонки'),
(19, 'Микшер'),
(20, 'Видеомикшер'),
(100, '3D-принтер'),
(101, 'Сканер'),
(102, 'Наушники'),
(103, 'Веб-камера'),
(104, 'Интерактивный стол');

-- --------------------------------------------------------

--
-- Структура таблицы `roles`
--

CREATE TABLE `roles` (
  `id` int(11) NOT NULL,
  `name` varchar(50) CHARACTER SET utf8 NOT NULL,
  `description` varchar(255) CHARACTER SET utf8 DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `roles`
--

INSERT INTO `roles` (`id`, `name`, `description`) VALUES
(1, 'Admin', 'Администратор системы'),
(2, 'Dispatcher', 'Сотрудник учебной части'),
(3, 'Teacher', 'Преподаватель');

-- --------------------------------------------------------

--
-- Структура таблицы `room_conditions`
--

CREATE TABLE `room_conditions` (
  `id` int(11) NOT NULL,
  `name` varchar(50) CHARACTER SET utf8 NOT NULL,
  `description` varchar(255) CHARACTER SET utf8 DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `room_conditions`
--

INSERT INTO `room_conditions` (`id`, `name`, `description`) VALUES
(1, 'Исправна', 'Аудитория в рабочем состоянии'),
(2, 'Ремонт', 'Аудитория на ремонте'),
(3, 'Закрыта', 'Аудитория временно закрыта');

-- --------------------------------------------------------

--
-- Структура таблицы `room_types`
--

CREATE TABLE `room_types` (
  `id` int(11) NOT NULL,
  `name` varchar(100) CHARACTER SET utf8 NOT NULL,
  `description` varchar(255) CHARACTER SET utf8 DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `room_types`
--

INSERT INTO `room_types` (`id`, `name`, `description`) VALUES
(1, 'Учебный кабинет', 'Стандартный учебный кабинет'),
(2, 'Лаборатория', 'Лабораторное помещение'),
(3, 'Компьютерный класс', 'Класс с компьютерами'),
(4, 'Лекционный зал', 'Большой лекционный зал'),
(5, 'Спортивный зал', 'Спортивное помещение');

-- --------------------------------------------------------

--
-- Структура таблицы `schedule`
--

CREATE TABLE `schedule` (
  `id` int(11) NOT NULL,
  `booking_id` int(11) DEFAULT NULL,
  `classroom_id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `event_date` date NOT NULL,
  `start_time` time NOT NULL,
  `end_time` time NOT NULL,
  `pair_number` int(11) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `schedule`
--

INSERT INTO `schedule` (`id`, `booking_id`, `classroom_id`, `teacher_id`, `event_date`, `start_time`, `end_time`, `pair_number`, `created_at`, `updated_at`) VALUES
(1, NULL, 2, 1, '2026-06-18', '09:45:00', '11:20:00', 2, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(2, NULL, 5, 2, '2026-06-19', '13:35:00', '15:10:00', 4, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(3, NULL, 8, 3, '2026-06-20', '15:20:00', '16:55:00', 5, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(4, NULL, 11, 4, '2026-06-21', '08:00:00', '09:35:00', 1, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(5, NULL, 16, 5, '2026-06-22', '11:30:00', '13:05:00', 3, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(6, NULL, 10, 6, '2026-07-01', '08:00:00', '09:35:00', 1, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(7, NULL, 13, 7, '2026-07-02', '11:30:00', '13:05:00', 3, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(8, NULL, 19, 8, '2026-07-03', '09:45:00', '11:20:00', 2, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(9, NULL, 21, 9, '2026-07-04', '13:35:00', '15:10:00', 4, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(10, NULL, 23, 10, '2026-07-05', '15:20:00', '16:55:00', 5, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(11, NULL, 1, 11, '2026-07-10', '08:00:00', '09:35:00', 1, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(12, NULL, 3, 12, '2026-07-11', '11:30:00', '13:05:00', 3, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(13, NULL, 4, 13, '2026-07-12', '09:45:00', '11:20:00', 2, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(14, NULL, 6, 14, '2026-07-13', '13:35:00', '15:10:00', 4, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(15, NULL, 7, 15, '2026-07-14', '15:20:00', '16:55:00', 5, '2026-06-19 21:18:50', '2026-06-19 21:18:50'),
(16, 105, 102, 101, '2026-08-06', '09:45:00', '11:20:00', 2, '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(17, 105, 102, 101, '2026-08-13', '09:45:00', '11:20:00', 2, '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(18, 105, 102, 101, '2026-08-20', '09:45:00', '11:20:00', 2, '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(19, 105, 102, 101, '2026-08-27', '09:45:00', '11:20:00', 2, '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(20, 105, 102, 101, '2026-09-03', '09:45:00', '11:20:00', 2, '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(21, 107, 100, 100, '2026-08-08', '08:00:00', '09:35:00', 1, '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(22, 111, 100, 100, '2026-08-12', '13:35:00', '15:10:00', 4, '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(23, 114, 101, 103, '2026-08-15', '15:20:00', '16:55:00', 5, '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(24, NULL, 103, 102, '2026-08-16', '11:30:00', '13:05:00', 3, '2026-06-19 21:23:07', '2026-06-19 21:23:07'),
(25, NULL, 103, 100, '2026-08-17', '08:00:00', '09:35:00', 1, '2026-06-19 21:23:07', '2026-06-19 21:23:07');

-- --------------------------------------------------------

--
-- Структура таблицы `system_settings`
--

CREATE TABLE `system_settings` (
  `id` int(11) NOT NULL,
  `setting_key` varchar(100) CHARACTER SET utf8 NOT NULL,
  `setting_value` text COLLATE utf8mb4_unicode_ci,
  `description` varchar(255) CHARACTER SET utf8 DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `system_settings`
--

INSERT INTO `system_settings` (`id`, `setting_key`, `setting_value`, `description`) VALUES
(1, 'pair_1_start', '08:00', 'Начало 1-й пары'),
(2, 'pair_1_end', '09:35', 'Конец 1-й пары'),
(3, 'pair_2_start', '09:45', 'Начало 2-й пары'),
(4, 'pair_2_end', '11:20', 'Конец 2-й пары'),
(5, 'pair_3_start', '11:30', 'Начало 3-й пары'),
(6, 'pair_3_end', '13:05', 'Конец 3-й пары'),
(7, 'pair_4_start', '13:35', 'Начало 4-й пары'),
(8, 'pair_4_end', '15:10', 'Конец 4-й пары'),
(9, 'pair_5_start', '15:20', 'Начало 5-й пары'),
(10, 'pair_5_end', '16:55', 'Конец 5-й пары');

-- --------------------------------------------------------

--
-- Структура таблицы `teachers`
--

CREATE TABLE `teachers` (
  `id` int(11) NOT NULL,
  `full_name` varchar(150) CHARACTER SET utf8 NOT NULL,
  `academic_degree` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `department_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `teachers`
--

INSERT INTO `teachers` (`id`, `full_name`, `academic_degree`, `department_id`) VALUES
(1, 'Иванов Иван Иванович', 'Кандидат наук', 1),
(2, 'Петрова Мария Сергеевна', NULL, 2),
(3, 'Сидоров Алексей Петрович', 'Доктор наук', 3),
(4, 'Козлова Елена Викторовна', 'Кандидат наук', 1),
(5, 'Михайлов Дмитрий Сергеевич', NULL, 2),
(6, 'Смирнова Ольга Владимировна', 'Доктор наук', 4),
(7, 'Васильев Петр Александрович', 'Кандидат наук', 5),
(8, 'Федорова Анна Игоревна', NULL, 1),
(9, 'Николаев Сергей Викторович', 'Кандидат наук', 3),
(10, 'Александрова Татьяна Борисовна', NULL, 2),
(11, 'Егоров Илья Дмитриевич', 'Доктор наук', 4),
(12, 'Морозова Наталья Юрьевна', 'Кандидат наук', 5),
(13, 'Волков Андрей Сергеевич', NULL, 1),
(14, 'Соловьева Екатерина Павловна', 'Кандидат наук', 2),
(15, 'Зайцев Максим Владимирович', NULL, 3),
(100, 'Новикова Ольга Петровна', 'Кандидат наук', 2),
(101, 'Зайцев Андрей Владимирович', NULL, 1),
(102, 'Григорьева Ирина Сергеевна', 'Доктор наук', 3),
(103, 'Титов Максим Александрович', 'Кандидат наук', 2);

-- --------------------------------------------------------

--
-- Структура таблицы `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `login` varchar(100) CHARACTER SET utf8 NOT NULL,
  `password_hash` varchar(255) CHARACTER SET utf8 NOT NULL,
  `role_id` int(11) NOT NULL,
  `teacher_id` int(11) DEFAULT NULL,
  `is_blocked` bit(1) NOT NULL DEFAULT b'0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `last_login` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `users`
--

INSERT INTO `users` (`id`, `login`, `password_hash`, `role_id`, `teacher_id`, `is_blocked`, `created_at`, `last_login`) VALUES
(1, 'admin', '$2a$12$aRp2dcM/ddlVpazKXkknDeLjydWMEj5/bvx6yiLg2mL9fRCqQYaXm', 1, NULL, b'0', '2026-06-19 21:18:58', '2026-06-20 19:59:44'),
(2, 'dispatcher', '$2a$12$Y2tBmWqUqqItSdmh8j4gB.q9t8sEUY8o78CJlGVAoC0HYhTpjPXQm', 2, NULL, b'0', '2026-06-19 21:18:58', NULL),
(3, 'teacher1', '$2a$12$lr.T7NvGY5D1qlPTYnv7Surj8JF50T3zmjlJS1auaePzDRGRbCZkS', 3, 1, b'0', '2026-06-19 21:18:59', NULL);

-- --------------------------------------------------------

--
-- Структура таблицы `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `audit_logs`
--
ALTER TABLE `audit_logs`
  ADD PRIMARY KEY (`id`),
  ADD KEY `user_id` (`user_id`);

--
-- Индексы таблицы `bookings`
--
ALTER TABLE `bookings`
  ADD PRIMARY KEY (`id`),
  ADD KEY `teacher_id` (`teacher_id`),
  ADD KEY `classroom_id` (`classroom_id`),
  ADD KEY `requested_room_type_id` (`requested_room_type_id`),
  ADD KEY `preferred_building_id` (`preferred_building_id`),
  ADD KEY `status_id` (`status_id`),
  ADD KEY `approved_by` (`approved_by`),
  ADD KEY `approved_room_id` (`approved_room_id`);

--
-- Индексы таблицы `booking_statuses`
--
ALTER TABLE `booking_statuses`
  ADD PRIMARY KEY (`id`);

--
-- Индексы таблицы `buildings`
--
ALTER TABLE `buildings`
  ADD PRIMARY KEY (`id`);

--
-- Индексы таблицы `classrooms`
--
ALTER TABLE `classrooms`
  ADD PRIMARY KEY (`id`),
  ADD KEY `building_id` (`building_id`),
  ADD KEY `room_type_id` (`room_type_id`),
  ADD KEY `condition_id` (`condition_id`),
  ADD KEY `department_id` (`department_id`);

--
-- Индексы таблицы `classroom_equipment`
--
ALTER TABLE `classroom_equipment`
  ADD PRIMARY KEY (`classroom_id`,`equipment_id`),
  ADD KEY `equipment_id` (`equipment_id`);

--
-- Индексы таблицы `departments`
--
ALTER TABLE `departments`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_dept_head` (`head_id`);

--
-- Индексы таблицы `equipment`
--
ALTER TABLE `equipment`
  ADD PRIMARY KEY (`id`);

--
-- Индексы таблицы `roles`
--
ALTER TABLE `roles`
  ADD PRIMARY KEY (`id`);

--
-- Индексы таблицы `room_conditions`
--
ALTER TABLE `room_conditions`
  ADD PRIMARY KEY (`id`);

--
-- Индексы таблицы `room_types`
--
ALTER TABLE `room_types`
  ADD PRIMARY KEY (`id`);

--
-- Индексы таблицы `schedule`
--
ALTER TABLE `schedule`
  ADD PRIMARY KEY (`id`),
  ADD KEY `booking_id` (`booking_id`),
  ADD KEY `classroom_id` (`classroom_id`),
  ADD KEY `teacher_id` (`teacher_id`);

--
-- Индексы таблицы `system_settings`
--
ALTER TABLE `system_settings`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `setting_key` (`setting_key`);

--
-- Индексы таблицы `teachers`
--
ALTER TABLE `teachers`
  ADD PRIMARY KEY (`id`),
  ADD KEY `department_id` (`department_id`);

--
-- Индексы таблицы `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `login` (`login`),
  ADD KEY `role_id` (`role_id`),
  ADD KEY `teacher_id` (`teacher_id`);

--
-- Индексы таблицы `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `audit_logs`
--
ALTER TABLE `audit_logs`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT для таблицы `bookings`
--
ALTER TABLE `bookings`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=119;

--
-- AUTO_INCREMENT для таблицы `booking_statuses`
--
ALTER TABLE `booking_statuses`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT для таблицы `buildings`
--
ALTER TABLE `buildings`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT для таблицы `classrooms`
--
ALTER TABLE `classrooms`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=104;

--
-- AUTO_INCREMENT для таблицы `departments`
--
ALTER TABLE `departments`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT для таблицы `equipment`
--
ALTER TABLE `equipment`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=105;

--
-- AUTO_INCREMENT для таблицы `roles`
--
ALTER TABLE `roles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT для таблицы `room_conditions`
--
ALTER TABLE `room_conditions`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT для таблицы `room_types`
--
ALTER TABLE `room_types`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT для таблицы `schedule`
--
ALTER TABLE `schedule`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT для таблицы `system_settings`
--
ALTER TABLE `system_settings`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT для таблицы `teachers`
--
ALTER TABLE `teachers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=104;

--
-- AUTO_INCREMENT для таблицы `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `audit_logs`
--
ALTER TABLE `audit_logs`
  ADD CONSTRAINT `audit_logs_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE SET NULL;

--
-- Ограничения внешнего ключа таблицы `bookings`
--
ALTER TABLE `bookings`
  ADD CONSTRAINT `bookings_ibfk_1` FOREIGN KEY (`teacher_id`) REFERENCES `teachers` (`id`),
  ADD CONSTRAINT `bookings_ibfk_2` FOREIGN KEY (`classroom_id`) REFERENCES `classrooms` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `bookings_ibfk_3` FOREIGN KEY (`requested_room_type_id`) REFERENCES `room_types` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `bookings_ibfk_4` FOREIGN KEY (`preferred_building_id`) REFERENCES `buildings` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `bookings_ibfk_5` FOREIGN KEY (`status_id`) REFERENCES `booking_statuses` (`id`),
  ADD CONSTRAINT `bookings_ibfk_6` FOREIGN KEY (`approved_by`) REFERENCES `users` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `bookings_ibfk_7` FOREIGN KEY (`approved_room_id`) REFERENCES `classrooms` (`id`) ON DELETE SET NULL;

--
-- Ограничения внешнего ключа таблицы `classrooms`
--
ALTER TABLE `classrooms`
  ADD CONSTRAINT `classrooms_ibfk_1` FOREIGN KEY (`building_id`) REFERENCES `buildings` (`id`),
  ADD CONSTRAINT `classrooms_ibfk_2` FOREIGN KEY (`room_type_id`) REFERENCES `room_types` (`id`),
  ADD CONSTRAINT `classrooms_ibfk_3` FOREIGN KEY (`condition_id`) REFERENCES `room_conditions` (`id`),
  ADD CONSTRAINT `classrooms_ibfk_4` FOREIGN KEY (`department_id`) REFERENCES `departments` (`id`) ON DELETE SET NULL;

--
-- Ограничения внешнего ключа таблицы `classroom_equipment`
--
ALTER TABLE `classroom_equipment`
  ADD CONSTRAINT `classroom_equipment_ibfk_1` FOREIGN KEY (`classroom_id`) REFERENCES `classrooms` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `classroom_equipment_ibfk_2` FOREIGN KEY (`equipment_id`) REFERENCES `equipment` (`id`) ON DELETE CASCADE;

--
-- Ограничения внешнего ключа таблицы `departments`
--
ALTER TABLE `departments`
  ADD CONSTRAINT `fk_dept_head` FOREIGN KEY (`head_id`) REFERENCES `teachers` (`id`) ON DELETE SET NULL;

--
-- Ограничения внешнего ключа таблицы `schedule`
--
ALTER TABLE `schedule`
  ADD CONSTRAINT `schedule_ibfk_1` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `schedule_ibfk_2` FOREIGN KEY (`classroom_id`) REFERENCES `classrooms` (`id`),
  ADD CONSTRAINT `schedule_ibfk_3` FOREIGN KEY (`teacher_id`) REFERENCES `teachers` (`id`);

--
-- Ограничения внешнего ключа таблицы `teachers`
--
ALTER TABLE `teachers`
  ADD CONSTRAINT `teachers_ibfk_1` FOREIGN KEY (`department_id`) REFERENCES `departments` (`id`) ON DELETE SET NULL;

--
-- Ограничения внешнего ключа таблицы `users`
--
ALTER TABLE `users`
  ADD CONSTRAINT `users_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`),
  ADD CONSTRAINT `users_ibfk_2` FOREIGN KEY (`teacher_id`) REFERENCES `teachers` (`id`) ON DELETE SET NULL;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
