FROM openjdk:11
COPY ./target /tmp
WORKDIR /tmp
EXPOSE 8080
ENTRYPOINT ["java","-jar", "server-0.7-jar-with-dependencies.jar"]