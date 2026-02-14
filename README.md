🎬 CineLog – Sistema de Reseñas de Películas | ASP.NET MVC + Identity + OpenAI
📌 Descripción

Aplicación web desarrollada en ASP.NET MVC con .NET y C#, orientada a una comunidad de usuarios que desean explorar películas y publicar reseñas.

El sistema permite a los visitantes navegar el catálogo de películas y consultar información general, mientras que los usuarios registrados pueden calificar, comentar y administrar su perfil.
Además, se incorporó inteligencia artificial para generar spoilers automáticos desde la ficha de cada película.

La aplicación cuenta con un panel administrativo protegido por autenticación y roles.

🚀 Funcionalidades principales
🎞️ Portal público

Listado de películas con paginado

Filtros por género y plataforma

Buscador por título

Vista de detalle de película

Visualización de reseñas de otros usuarios

Generación de spoiler automático mediante IA (modal dinámico)

👤 Usuarios registrados

Registro e inicio de sesión

Gestión de perfil

Publicación de reseñas

Calificación por estrellas (1 a 5)

Historial de reseñas realizadas

🔐 Panel administrador

Acceso exclusivo mediante rol Admin:

Alta de películas

Modificación de películas

Eliminación de películas

Gestión de géneros

Gestión de plataformas

Control total del contenido del sistema

🤖 Integración de Inteligencia Artificial

Se integró la API de OpenAI para generar automáticamente spoilers de películas.

Características:

Botón "Generar Spoiler con IA"

Modal interactivo sin recargar la página

Comunicación asíncrona mediante Fetch/AJAX

Servicio dedicado (LlmService) para consumo de la API

🛠️ Tecnologías utilizadas

C#

.NET

ASP.NET MVC

Entity Framework Core

ASP.NET Identity (autenticación y roles)

SQL Server

HTML5

CSS3

Bootstrap

JavaScript

Fetch API / AJAX

API de OpenAI

🔐 Seguridad

El sistema implementa seguridad basada en:

Autenticación mediante ASP.NET Identity

Autorización por roles ([Authorize])

Restricción de acceso a controladores administrativos

Protección de formularios con AntiForgeryToken

📐 Arquitectura

La aplicación sigue el patrón MVC (Model – View – Controller):

Model: entidades de dominio y persistencia (Entity Framework)
View: interfaz de usuario con Razor Views
Controller: manejo de peticiones, lógica de flujo y validaciones

También se implementó una capa de servicios para la integración con IA:

LlmService → Encapsula la comunicación con OpenAI.

🗄️ Base de datos

Base de datos SQL Server

Persistencia completa de:

Películas

Géneros

Plataformas

Usuarios

Reseñas

Roles

El sistema utiliza Entity Framework Core con migraciones para la generación y actualización de la base de datos.

🌐 Experiencia de usuario

La interfaz fue diseñada con enfoque en experiencia visual:

Diseño responsive

Modal dinámico para IA

Sistema de estrellas interactivo

Navegación simple

Navbar persistente

Feedback visual al usuario

👨‍🏫 Contexto académico

Proyecto desarrollado como práctica integral aplicando conceptos avanzados de desarrollo web en .NET:

MVC

Autenticación y autorización

Manejo de usuarios

Consumo de APIs externas

Comunicación asíncrona

Arquitectura por capas y servicios
