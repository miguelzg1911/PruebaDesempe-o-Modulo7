TalentoPlus S.A.S. - Sistema de Gestión de Empleados
📋 Descripción del Proyecto

Sistema completo para la gestión de empleados desarrollado con ASP.NET Core y MySQL. Incluye aplicación web para administradores de RRHH y API REST para empleados.
🚀 Características Principales
🔐 Autenticación y Autorización

    Aplicación Web (Admin): ASP.NET Core Identity

    API REST (Empleados): JWT Authentication

    Roles: Admin (acceso completo) y Empleado (solo su información)

📊 Dashboard de RRHH

    Tarjetas de información en tiempo real

    Procesamiento de consultas en lenguaje natural

    Estadísticas de empleados

📄 Gestión de Empleados

    CRUD completo de empleados

    Importación desde archivos Excel

    Generación de Hojas de Vida en PDF

📱 API REST

    Endpoints públicos (sin autenticación)

    Endpoints protegidos con JWT

    Autoregistro de empleados

    Consulta de información personal

🏗️ Arquitectura
text

TalentoPlus/
├── TalentoPlus.Domain/          # Entidades y interfaces
├── TalentoPlus.Application/     # Lógica de negocio y DTOs
├── TalentoPlus.Infrastructure/  # Acceso a datos, repositorios
├── TalentoPlus.API/             # Controladores y endpoints

🛠️ Tecnologías Utilizadas

    Backend: ASP.NET Core 8, Entity Framework Core

    Base de datos: MySQL (Pomelo.EntityFrameworkCore.MySql)

    Autenticación: JWT, BCrypt para hashing

    PDF Generation: QuestPDF

    Excel Processing: EPPlus

⚙️ Configuración e Instalación
Prerrequisitos

    .NET 8 SDK

    MySQL Server 8.0+

    MySQL Workbench (opcional, para gestión)

Paso 1: Configurar MySQL
sql

-- Conectar a MySQL como root
mysql -u root -p

-- Crear usuario (opcional)
CREATE USER 'talentoplus'@'localhost' IDENTIFIED BY 'StrongPass123!';
GRANT ALL PRIVILEGES ON TalentoPlus.* TO 'talentoplus'@'localhost';
FLUSH PRIVILEGES;

Clonar y configurar el proyecto
bash

# Clonar repositorio
git clone <https://github.com/miguelzg1911/PruebaDesempe-o-Modulo7>
cd PruebaModulo7

# Navegar al proyecto API
cd src/TalentoPlus.API

Paso 3: Configurar conexión MySQL

Crear archivo appsettings.Development.json en src/TalentoPlus.API:
json

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TalentoPlus;User=root;Password=tu_password_mysql;"
  },
  "Jwt": {
    "Key": "SuperSecretKeyMinimum32CharactersLong1234567890",
    "Issuer": "TalentoPlus",
    "Audience": "TalentoPlusUsers"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "tu-email@gmail.com",
    "Password": "tu-app-password",
    "From": "noreply@talentoplus.com"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

Paso 4: Instalar dependencias y aplicar migraciones
bash

# Asegúrate de estar en src/TalentoPlus.API
cd src/TalentoPlus.API

# Restaurar paquetes
dotnet restore

# Aplicar migraciones
dotnet ef database update --project ../TalentoPlus.Infrastructure/ --startup-project .

Paso 5: Ejecutar la aplicación
bash

# Ejecutar en modo desarrollo
dotnet run

# O para desarrollo con recarga automática
dotnet watch run

La aplicación estará disponible en:

    API: http://localhost:5020

    Swagger UI: http://localhost:5020/swagger

🔐 Credenciales de Acceso
Usuario Administrador (creado automáticamente)

Si no existe usuario admin, el primer registro se crea como Admin.

Para crear manualmente:
sql

-- Conectar a MySQL
mysql -u root -p TalentoPlus

-- Generar hash de 'Admin123!' (ejecutar en C#):
-- var hash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
-- Console.WriteLine(hash);

-- Insertar usuario admin (reemplaza 'tu_hash_aqui' con el hash real)
INSERT INTO Users (UserName, Email, PasswordHash, Role, Nombres, Apellidos) 
VALUES (
  'admin@talentoplus.com',
  'admin@talentoplus.com',
  '$2a$11$N9qo8uLOickgx2ZMRZoMye.7.7dC5YV6zQ8J.8JzQ8JzQ8JzQ8JzQ',
  'Admin',
  'Administrador',
  'Sistema'
);

O usar el endpoint de registro (primer usuario será Admin):
http

POST /api/auth/register
{
  "documento": "100000001",
  "nombres": "Admin",
  "apellidos": "Sistema",
  "email": "admin@talentoplus.com",
  "password": "Admin123!",
  "telefono": "3001234567",
  "departamentoId": 1
}

📚 Endpoints de la API
🔓 Endpoints Públicos (sin autenticación)

    GET /api/departamentos - Listar departamentos

    POST /api/auth/register - Registrar nuevo empleado

    POST /api/auth/login - Iniciar sesión

🔒 Endpoints Protegidos (requieren JWT)
Para Administradores (Role: Admin)

    GET /api/empleados - Listar todos los empleados

    GET /api/empleados/{id} - Obtener empleado específico

    POST /api/empleados - Crear nuevo empleado

    PUT /api/empleados/{id} - Actualizar empleado

    DELETE /api/empleados/{id} - Eliminar empleado

    POST /api/empleados/import-excel - Importar desde Excel

    GET /api/empleados/{id}/pdf - Generar PDF de cualquier empleado

    GET /api/dashboard/stats - Estadísticas del dashboard

    POST /api/dashboard/ask - Consultas en lenguaje natural

Para Empleados (Role: Empleado o Admin)

    GET /api/empleados/me - Ver mi información

    GET /api/empleados/me/pdf - Descargar mi Hoja de Vida

📊 Dashboard de Inteligencia Artificial
Preguntas Soportadas

El sistema puede responder a consultas en lenguaje natural como:

    "¿Cuántos empleados hay en total?"

    "¿Cuántos empleados están activos/inactivos?"

    "¿Cuál es el salario promedio?"

    "Mostrar distribución por departamentos"

    "¿Qué porcentaje de empleados está activo?"

Implementación

    Versión actual: Sistema basado en reglas heurísticas

    Para producción: Integrable con OpenAI/Gemini API

📄 Generación de PDF
Características de la Hoja de Vida

    Datos personales del empleado

    Información laboral (cargo, salario, fecha ingreso)

    Nivel educativo y perfil profesional

    Departamento asignado

    Datos de contacto

Uso
http

GET /api/empleados/me/pdf        # Para empleados (su propio PDF)
GET /api/empleados/{id}/pdf      # Para administradores (cualquier empleado)

📈 Importación desde Excel
Formato Requerido

El archivo Excel debe tener estas columnas (en orden):

    Documento

    Nombres

    Apellidos

    FechaNacimiento

    Direccion

    Telefono

    Email

    Cargo

    Salario

    FechaIngreso

    Estado

    NivelEducativo

    PerfilProfesional

    Departamento

Ejemplo
http

POST /api/empleados/import-excel
Content-Type: multipart/form-data
Body: file=[tu-archivo.xlsx]

📁 Estructura de la Base de Datos MySQL
Tablas principales:
sql

-- Ver estructura de tablas
USE TalentoPlus;
SHOW TABLES;

-- Ver datos de ejemplo
SELECT * FROM Departamentos;
SELECT COUNT(*) as TotalEmpleados FROM Empleados;
SELECT Estado, COUNT(*) as Cantidad FROM Empleados GROUP BY Estado;

🔧 Solución de Problemas Comunes con MySQL
Problema 1: Error de conexión "Unable to connect to any of the specified MySQL hosts"
bash

# Verificar que MySQL esté corriendo
sudo service mysql status

# Probar conexión manual
mysql -u root -p -h localhost

# Verificar permisos del usuario
SELECT host, user FROM mysql.user;

Problema 2: Error "The table 'table_name' already exists" en migraciones
bash

# Eliminar y recrear base de datos
mysql -u root -p -e "DROP DATABASE TalentoPlus; CREATE DATABASE TalentoPlus;"

# Recrear migraciones
dotnet ef migrations remove --project ../TalentoPlus.Infrastructure/
dotnet ef migrations add InitialCreate --project ../TalentoPlus.Infrastructure/
dotnet ef database update --project ../TalentoPlus.Infrastructure/

Problema 3: Error de collation o encoding
sql

-- Cambiar collation de la base de datos
ALTER DATABASE TalentoPlus CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Ver collation actual
SELECT @@character_set_database, @@collation_database;

Problema 4: Error con fechas "0001-01-01"
sql

-- Actualizar fechas incorrectas
UPDATE Empleados 
SET FechaNacimiento = '1990-01-01'
WHERE FechaNacimiento = '0001-01-01';

🧪 Pruebas
Ejecutar pruebas unitarias
bash

# Desde la carpeta raíz de la solución
dotnet test

Pruebas manuales con Postman

Importar la colección TalentoPlus.postman_collection.json incluida en el proyecto.
🚀 Despliegue en Producción (MySQL)
Recomendaciones para producción:

    MySQL en producción:

        Usar MySQL 8.0+

        Configurar backups automáticos

        Usar réplicas para lectura

    Conexión segura:

json

"ConnectionStrings": {
  "DefaultConnection": "Server=servidor-produccion;Database=TalentoPlus;User=usuario_seguro;Password=contraseña_fuerte;SslMode=Required"
}

    Optimizaciones MySQL:

sql

-- Crear índices para mejor performance
CREATE INDEX idx_empleados_estado ON Empleados(Estado);
CREATE INDEX idx_empleados_departamento ON Empleados(DepartamentoId);
CREATE UNIQUE INDEX idx_empleados_documento ON Empleados(Documento);
CREATE UNIQUE INDEX idx_empleados_email ON Empleados(Email);

📋 Checklist de Implementación
✅ Completado:

    CRUD completo de empleados

    Autenticación JWT con BCrypt

    Importación desde Excel (EPPlus)

    Generación de PDFs (QuestPDF)

    Dashboard con estadísticas

    Procesamiento de lenguaje natural básico

    API REST con endpoints públicos/protegidos

    Base de datos MySQL configurada

🔄 Pendiente (opcional para la prueba):

    Interfaz web completa para admin

    Integración con IA real (Gemini/OpenAI)

    Sistema de correos real con SMTP

    Dockerización completa

    Pruebas unitarias e integración

📞 Soporte

Para problemas técnicos:

    Verificar logs de la aplicación:

bash

cd src/TalentoPlus.API
dotnet run 2>&1 | tee app.log

    Verificar conexión MySQL:

bash

mysql -u root -p -e "SHOW PROCESSLIST;" TalentoPlus

    Reiniciar servicios:

bash

# Reiniciar MySQL
sudo service mysql restart

# Limpiar y reconstruir proyecto
dotnet clean
dotnet build
