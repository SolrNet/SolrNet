# Using Docker for Running Solr Instances

**TL;DR**: Scroll down for instructions on how to start multiple versions of Solr using Docker

Getting a Solr instance is usually one of the first steps whenever you want to start development of an enterprise search application. Configuring Solr is not hard at all, for example if you wanted to install [Solr 7 you could follow these instructions](https://lucene.apache.org/solr/guide/7_0/installing-solr.html) or if you needed an older version, i.e. [5.5 you can follow the steps located in this archived reference guide](https://archive.apache.org/dist/lucene/solr/ref-guide/apache-solr-ref-guide-5.5.pdf).

And the steps are simple, mainly making sure you have Java installed, download a zip, extract it and run a command like
```sh
$ bin/solr start
```

However, you need to follow the steps for every version of Solr and that can take some time. Luckily, there is a better way, using Docker.

If you have not heard of Docker, [you can read more here](https://www.docker.com/), but in esscence it is a technology that provides an additional level of abstraction and automation by allowing independent *containers* to run in your machine. In layman terms, like a virtual machine but better and easier to use as you can start a Solr container using a single command; Docker takes care of downloading the required image, and starting it. You can also run multiple containers side by side with different versions of Solr. 

# Supported Solr Versions in Docker Hub
Currently the available versions of Solr are 5.5.5 to 7.1.0, but as newer versions are released then newer images are created. You can find the up to date list in [Docker's Official Repository](https://hub.docker.com/_/solr/).

![Solr images available in Docker Hub](/Documentation/images/solr-docker-tags.png)

# Instructions on How to Set up Docker
Setting up Docker is simple. There are only a few considerations to make
- Select which type of Docker you want, Community or Enterprise. Most likely Community is the way to go if you are in development and testing.
- Are you in a supported platform? Please refer to the [installation documentation](https://docs.docker.com/engine/installation/#desktop) to confirm.  If all is good, download and install. To make it quick for you, if you have a version of Windows less than 10, you cannot use these editions of Docker. Instead you need Docker Toolbox.
- Docker Toolbox is a legacy desktop solutions for older versions of Windows and Mac. [Download](https://docs.docker.com/toolbox/toolbox_install_windows/) and install.
- Now open the Docker Terminal

![Start the terminal](/Documentation/images/docker-qs-search.png)

- And you are ready to start some containers.

![Docker Terminal](/Documentation/images/docker-terminal.png)

The ip displayed in your terminal is used to access your Solr instance

*Troubleshooting advice:*
Under certain circumstances the VirtualBox driver that is required by Docker does not start. Confirm by running the first command below and starting the driver

```
sc.exe query vboxdrv
sc.exe start vboxdrv
```

# Starting a Solr Instance with Docker
Now you can request Docker to find a particular image, download and start. This is the way to start the default Solr image:

```
$ docker run --name my_solr -d -p 8983:8983 -t sol
```

Once the image has been downloaded, simply access it by the IP displayed in Docker terminal and the mapped port, in this case 8983 was mapped on both ends

```
http://192.168.99.100:8983/
```

Hello Solr 7!

![Solr 7](/Documentation/images/solr-7.png)

If you want to download a different version from the supported list, simply add the tag to specify which one of the versions of Solr available in Docker Hub you want to use. Docker will download what's missing on the container to run the other version of Solr.  Wait a little bit and it will start. 

```
$ docker run --name solr_five -d -p 8983:8983 -t solr:5.5.5
```

![Solr 5.5](/Documentation/images/solr-5-download.png)

Hello Solr 5.5!

![Solr 5.5](/Documentation/images/solr-5.png)

You can stop using:
```
docker stop test_solr
```
