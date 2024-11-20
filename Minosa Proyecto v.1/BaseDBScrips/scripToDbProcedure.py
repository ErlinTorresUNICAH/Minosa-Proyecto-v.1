#Este scrip en python te permite extraer todos los procedures almacenados de una base de datos SQL Server y guardarlos en un archivo .sql
#Configuras la conexion a tu base y ejecutas, recuerda tener instalado pyodbc y python.
# Para ejecutar solamente ve a la carpeta donde se encuentra el archivo y ejecuta el comando | python scripToDbProcedure.py | o | python3 scripToDbProcedure.py |
import pyodbc

# Configura tu conexión a la base de datos
conn = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost;DATABASE=Testv1_MinosaProyecto;Trusted_Connection=Yes;TrustServerCertificate=Yes')
cursor = conn.cursor()

# Consulta para obtener el código de los procedimientos almacenados
query = """
SELECT 
    OBJECT_SCHEMA_NAME(object_id) AS SchemaName,
    OBJECT_NAME(object_id) AS ProcedureName,
    definition
FROM sys.sql_modules
WHERE OBJECTPROPERTY(object_id, 'IsProcedure') = 1
"""

cursor.execute(query)
procedures = cursor.fetchall()

# Conjunto para rastrear nombres únicos de procedimientos
procedures_seen = set()

with open("ProcedimientosAlmacenados.sql", "w", encoding="utf-8") as file:
    for row in procedures:
        schema_name = row.SchemaName
        procedure_name = row.ProcedureName
        definition = row.definition.strip()  # Remueve espacios innecesarios

        # Construye el identificador completo del procedimiento
        full_procedure_name = f"{schema_name}.{procedure_name}"

        # Verifica si el procedimiento ya ha sido visto
        if full_procedure_name not in procedures_seen:
            file.write(f"-- PROCEDIMIENTO: [{schema_name}].[{procedure_name}]\n")
            file.write(f"{definition}\n")  # Escribe solo la definición tal como está
            file.write("GO\n\n")
            file.write("\n\n")
            file.write("\n\n")
            file.write("\n\n")
            # Añade el nombre del procedimiento al conjunto para evitar repeticiones
            procedures_seen.add(full_procedure_name)

cursor.close()
conn.close()
