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

# Guarda cada procedimiento en un archivo o uno solo como prefieras
with open("procedimientos.sql", "w", encoding="utf-8") as file:
    for row in procedures:
        schema_name = row.SchemaName
        procedure_name = row.ProcedureName
        definition = row.definition

        # Manejar posibles ALTER PROCEDURE y convertirlo a CREATE PROCEDURE
        definition = definition.replace("ALTER PROCEDURE", "CREATE PROCEDURE")

        file.write(f"CREATE PROCEDURE [{schema_name}].[{procedure_name}]\n{definition}\nGO\n\n")

cursor.close()
conn.close()
