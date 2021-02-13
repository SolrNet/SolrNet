FROM openjdk:8-jre
COPY . /solr
WORKDIR /solr
# for some reason setting maxFormContentSize in jetty.xml doesn't work
CMD java -Dorg.eclipse.jetty.server.Request.maxFormContentSize=5000000 -jar start.jar
