1. Crear el archivo docker-compose.yml
  Crea un archivo llamado docker-compose.yml en un directorio vacío y pega el siguiente contenido. Esta configuración utiliza imágenes oficiales y expone los puertos necesarios para que tus aplicaciones .NET puedan conectarse.
    ###########################################################################  

    version: '3.8'

    services:
      zookeeper:
        image: confluentinc/cp-zookeeper:7.5.0
        container_name: zookeeper
        hostname: zookeeper
        ports:
          - "2181:2181"
        environment:
          ZOOKEEPER_CLIENT_PORT: 2181
          ZOOKEEPER_TICK_TIME: 2000

      kafka:
        image: confluentinc/cp-kafka:7.5.0
        container_name: kafka
        hostname: kafka
        ports:
          - "9092:9092"
        depends_on:
          - zookeeper
        environment:
          KAFKA_BROKER_ID: 1
          KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
          # Advertencia: Esta es la dirección que tus aplicaciones .NET usarán para conectarse.
          KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:29092,PLAINTEXT_HOST://localhost:9092
          KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT
          KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT
          KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1

    ############################################################################

2. Iniciar los contenedores
  Abre tu terminal en el mismo directorio donde guardaste el archivo docker-compose.yml y ejecuta el siguiente comando (TAMBIEN PUEDES HACERLO DESDE LA TERMINAL DE DOCKER DESKTOP):

    ###

    docker-compose up -d

    ###

  Esto descargará las imágenes necesarias y levantará los contenedores de Zookeeper y Kafka en segundo plano.

3. Verificar que los contenedores estén corriendo

    ###

    docker ps

    ###
    
4. Conexión desde tu Aplicación .NET

  Una vez que Kafka esté corriendo, la configuración que debes usar en tus proyectos de .NET (OrderProducer y OrderProcessor) será:

  BootstrapServers: localhost:9092

  Nota: no es algo que puedas ver como un servicio en ejecución (desde una URL en el browser), pero si tus aplicaciones .NET pueden conectarse a Kafka usando la configuración anterior, entonces todo está funcionando correctamente.