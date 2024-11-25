import sys
import pandas as pd
from sklearn.ensemble import RandomForestClassifier
import pyodbc
import json
from datetime import datetime
from sqlalchemy import create_engine

def conectar_y_extraer_datos():
    connection_string = "mssql+pyodbc://localhost/Testv1_MinosaProyecto?driver=SQL+Server&trusted_connection=yes"
    engine = create_engine(connection_string)
    query = """
    SELECT ip, HoraPing, ResultadoPing
    FROM HistorialPings
    WHERE HoraPing >= DATEADD(DAY, -30, GETDATE()) -- Últimos 30 días
    """
    data = pd.read_sql_query(query, engine)
    return data

def preparar_datos(data):
    data['HoraPing'] = pd.to_datetime(data['HoraPing'])
    data['hora'] = data['HoraPing'].dt.hour
    data['dia_semana'] = data['HoraPing'].dt.dayofweek
    data['ResultadoPing'] = data['ResultadoPing'].astype(int)
    return data

def entrenar_modelo():
    datos = conectar_y_extraer_datos()
    datos_preparados = preparar_datos(datos)
    X = pd.get_dummies(datos_preparados[['hora', 'dia_semana', 'ip']], columns=['ip'])
    y = datos_preparados['ResultadoPing']
    model = RandomForestClassifier(random_state=42)
    model.fit(X, y)
    return model, X.columns

# Entrenar el modelo al inicio
modelo, columnas = entrenar_modelo()

def predecir(ip, ultima_hora_ping):
    try:
        # Convertir la fecha y hora al formato necesario
        hora_dt = datetime.strptime(ultima_hora_ping, "%d/%m/%Y %H:%M:%S")
        hora_int = hora_dt.hour
        # print(f"Procesando IP {ip} con hora base {hora_int}h")
        
        predicciones = []
        for hora in range(hora_int + 1, 24):
            nueva_entrada = pd.DataFrame({'hora': [hora], 'dia_semana': [pd.Timestamp.now().dayofweek], 'ip': [ip]})
            nueva_entrada = pd.get_dummies(nueva_entrada, columns=['ip'])
            nueva_entrada = nueva_entrada.reindex(columns=columnas, fill_value=0)
            prediccion = modelo.predict(nueva_entrada)
            estado = "Activo" if prediccion[0] == 1 else "Inactivo"
            # Convertir la hora a formato de 12 horas con AM/PM
            hora_formateada = datetime.strptime(str(hora), "%H").strftime("%I:%M %p")
            predicciones.append({"hora": hora_formateada, "estado": estado})
        return predicciones
    except Exception as e:
        print(f"Error al procesar la hora o IP: {e}")
        return []

if __name__ == "__main__":
    # Leer argumentos desde la línea de comandos
    ip = sys.argv[1]
    ultima_hora_ping = " ".join(sys.argv[2:])  # Combina los argumentos restantes en una sola cadena

    # Realizar la predicción
    resultados = predecir(ip, ultima_hora_ping)
    print(json.dumps(resultados, indent=2))
