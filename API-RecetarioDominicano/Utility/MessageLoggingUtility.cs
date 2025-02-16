using System.Net;

namespace API_RecetarioDominicano.Utility
{
    public class MessageLoggingUtility
    {
        private readonly Dictionary<string, string> errorDescriptions = new()
        {
            // General errors
            {"GENERAL_CONTACT", "\n\n Developers have been sent a notification of this failed call. They will contact you shortly. \n Below is our contact information for your records:\n\n Website: originsoftware.dev\n Email: Support@originsoftware.dev"}, // Se ha enviado una notificación a los desarrolladores sobre esta llamada fallida. Se pondrán en contacto contigo en breve. A continuación, se muestra nuestra información de contacto para que la tengas presente: Sitio web: originsoftware.dev Email: Support@originsoftware.dev
            {"GENERAL_UNEXPECTED_ERROR", "An unexpected error occurred."}, // Ha ocurrido un error inesperado.
            {"GENERAL_PAGE_NOT_FOUND", "The requested page was not found."}, // No se encontró la página solicitada.
            {"GENERAL_INVALID_REQUEST", "The request sent to the server is invalid."}, // La solicitud enviada al servidor es inválida.
            {"GENERAL_TIMEOUT_ERROR", "The request to the server timed out."}, // La solicitud al servidor ha expirado.
            {"GENERAL_CONNECTION_ERROR", "There was an error connecting to the server."}, // Se ordkujo un error al conectar con el servidor.

            // Create errors
            {"CREATE_EC01", "Attempting to create a record with a primary key that already exists."}, // Intento de crear un registro con una clave primaria que ya existe.
            {"CREATE_EC05", "Attempt to create a record with a foreign key that does not exist in the referenced table."}, // Intento de crear un registro con una clave externa que no existe en la tabla referenciada.
            {"CREATE_EC09", "Attempt to insert data with incorrect types into table columns."}, // Intento de insertar datos con tipos incorrectos en las columnas de la tabla.
            {"CREATE_EC13", "Attempt to insert data that exceeds the maximum size allowed for a particular column."}, // Intento de insertar datos que exceden el tamaño máximo permitido para una columna específica.
            {"CREATE_EC17", "Attempt to insert data in a format not supported by the column's data type."}, // Intento de insertar datos en un formato no compatible con el tipo de datos de la columna.
            {"CREATE_EC21", "Attempt to create a record without properly filled fields"}, // Intento de crear un registro sin campos completados correctamente

            // Read errors
            {"READ_ER02", "Attempt to read a record that does not exist."}, // Intento de leer un registro que no existe en la tabla.
            {"READ_ER10", "Attempt to read non-existing records" }, // Intento de leer registros no existente.

            // Permission error
            {"PERMISSION_EP06", "The user does not have sufficient permissions to read the requested data."}, // El usuario no tiene permisos suficientes para leer los datos solicitados.

            // Update errors
            {"UPDATE_EU03", "Attempt to update a record that does not exist."}, // Intento de actualizar un registro que no existe en la tabla.
            {"UPDATE_EU07", "Attempt to update a document that exceeds the allowed size limit for the collection."}, // Intento de actualizar un documento que excede el límite de tamaño permitido para la colección.
            {"UPDATE_EU11", "Attempt to update a record with a foreign key that does not exist in the referenced table." },
            // Delete errors
            {"DELETE_ED04", "Attempt to delete a record that does not exist."}, // Intento de eliminar un registro que no existe.
            {"DELETE_ED08", "The user does not have sufficient permissions to delete the registre."}, // El usuario no tiene suficiente permiso para eliminar el registro

            // Authentication errors
            {"AUTHENTICATION_AT01", "Authentication failed. Please ensure your email and password are correct."}, // .Falló la autenticación Por favor, asegúrate de que tu nombre de usuario y contraseña sean correctos
            {"AUTHENTICATION_AT02", "Client not found or not a member."},
            {"AUTHENTICATION_AT03", "Discrepancy detected in encrypted key values."},
            {"AUTHENTICATION_AT04", "Client already has an account please log in."},
            {"AUTHENTICATION_AT05", "Client is not a member."},

            // Session errors
            {"SESSION_EXPIRED", "Your session has expired. Please log in again to continue."}, //  Tu sesión ha expirado. Por favor, vuelve a iniciar sesión para continuar.

            // Form errors
            {"FORM_INVALID", "The submitted form is invalid. Please check and correct the fields marked."}, //  El formulario enviado es inválido. Por favor, verifica y corrige los campos marcados.

            // File errors
            {"FILE_NOT_FOUND", "The requested file could not be found."}, //  El archivo solicitado no se encuentra.

            // Server errors
            {"SERVER_ERROR", "An error occurred on the server. Please try again later."}, //  Ha ocurrido un error en el servidor. Por favor, inténtalo de nuevo más tarde.

            // User errors
            {"USER_INVALID_EMAIL", "The email address entered is not valid."}, // La dirección de correo electrónico ingresada no es válida.
            {"USER_PASSWORD_WEAK", "The password entered is too weak."}, // La contraseña ingresada es demasiado débil.
            {"USER_ACCOUNT_LOCKED", "Your account has been locked. Please contact support for assistance."}, // Su cuenta ha sido bloqueada. Por favor, póngase en contacto con el soporte para obtener ayuda.
            {"USER_EMAIL_ALREADY_EXISTS", "The email address is already associated with an existing account."}, // La dirección de correo electrónico ya está asociada con una cuenta existente.

            // Security errors
            {"SECURITY_CSRF_ATTACK", "CSRF attack detected. Please try again."}, // Se detectó un ataque CSRF. Por favor, inténtalo de nuevo.
            {"SECURITY_XSS_ATTACK", "XSS attack detected. Please refrain from entering invalid characters."}, // Se detectó un ataque XSS. Por favor, evite ingresar caracteres no válidos.

            // Data loading errors
            {"DATA_LOAD_FAILURE", "Failed to load data from the server."}, // Error al cargar datos desde el servidor.

            // Descripciones de errores
            {"ERROR_ARCHIVE_PRODUCT", "Failed to archive product."}, // Error al archivar el producto.
            {"INVALID_OPTIONS_PRICE_FORMAT", "The price format for options is invalid." },
            {"INVALID_OPTIONS", "The options provided are invalid or not allowed. Please check the values ​​and try again." },
            {"CANCELED", "The payment has been canceled."},

        };

        private readonly Dictionary<string, string> informationDescriptions = new()
        {
            {"SUCCESS_GET", "Data retrieval successful."}, // La recuperación de datos se realizó con éxito.
            {"SUCCESS_UPDATE", "Data update successful."}, // La actualización de datos se realizó con éxito.
            {"SUCCESS_DELETE", "Data deletion successful."}, // La eliminación de datos se realizó con éxito.
            {"SUCCESS_CREATE", "Data creation successful."}, // La creación de datos se realizó con éxito.
            {"SUCCESS_LOGIN", "Login successful."}, // Inicio de sesión exitoso.
            {"SUCCESS_LOGOUT", "Logout successful."}, // Cierre de sesión exitoso.
            {"SUCCESS_REGISTER", "Registration successful."}, // Registro exitoso.
            {"SUCCESS_UPLOAD", "File upload successful."}, // Carga de archivo exitosa.
            {"SUCCESS_DOWNLOAD", "File download successful."}, // Descarga de archivo exitosa.
            {"SUCCESS_OPERATION", "Operation successful."}, // Operación exitosa.
            {"SUCCESS_ARCHIVE_PRODUCT", "Product archived successfully."}, // Producto archivado correctamente.
        };


        public string GetErrorDescriptionC(string errorCode)
        {
            if (errorDescriptions.TryGetValue(errorCode, out string description))
            {
                return $"{errorCode} - {description} {errorDescriptions["CONTACT"]}\n Error: {errorCode}";
            }
            else
            {
                return "An unknown error occurred when trying to display error details. Please try again.";
            }
        }

        public string GetInformationDescription(string infoCode)
        {
            if (informationDescriptions.TryGetValue(infoCode, out string description))
            {
                return $"{infoCode} - {description}";
            }
            else
            {
                return "An unknown error occurred when trying to display error details. Please try again.";
            }
        }

        public string GetErrorDescription(string errorCode)
        {
            if (errorDescriptions.TryGetValue(errorCode, out string description))
            {
                return $"{errorCode} - {description}";
            }
            else
            {
                return "An unknown error occurred when trying to display error details. Please try again.";
            }
        }

        public static HttpResponseMessage HandleClientValidationErrors(string errorCode)
        {
            MessageLoggingUtility errorLog = new MessageLoggingUtility();
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(errorLog.GetErrorDescription(errorCode)),
                ReasonPhrase = "Unauthorized"
            };

            return response;
        }
    }
}
