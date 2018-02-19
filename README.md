TFG-Unity
==

Este proyecto tiene como objetivo estudiar diferentes controladores, como el wiimote, en Unity para demostrar las ventajas y desventajas frente a los controladores tradicionales.

Para esto, se realizaran distintas escenas en Unity demostrando las capacidades de los controladores usando librerias propias o encontradas por github.

## Contenido del proyecto

A continuación se explicara la conexión de los diferentes controladores, conforme los vaya haciendo, y las distintas escenas que los usen.

## Conexión Wiimote

Una vez realizado cada metodo deberia de detectarse automaticamente en play el primer wiimote conectado.

### Barra sensor Wii

Para poder hacer uso de la barra de sensor infrarroja de la wii en el PC se puede fabricar una barra casera o simplemente encender la wii y situar la barra de sensor que tengas conectada encima de la pantalla del pc.

### Windows

1. Pulsa el botón derecho del ratón en el icono de bluetooth de la barra de notificaciones.
2. Clickear unirse a una red de área personal.
3. Pulsando el botón de agregar dispositivo te saldrá una ventana que buscará automáticamente los dispositivos.
4. Mientras busca pulsa 1+2 en el wiimote hasta que aparezca, puede ser necesario mas de una búsqueda.
5. Seleccionar el wiimote en la ventana de búsqueda y dejar el pin vácio.

PD: Este método requiere desvincular el wiimote del pc y reconectarlo cada vez que se reinicie el PC.

Source: Experiencia y distintas fuentes.

### Linux

1. Instala cwiid con el siguiente comando de bash.
> sudo apt-get install libcwiid1 lswm wmgui wminput

2. Arranca el programa con el siguiente comando de bash.
> wmgui

3. Selecciona "connect" en el menu de archivo y pulsa 1+2 en el wiimote cuando te diga el programa.

Source: https://askubuntu.com/questions/705865/how-to-pair-wiimote-in-ubuntu-15-10

PD: Metodo no probado aún.

## WiimotePlayDemo

En esta escena solo se utilizan el hardware del wiimote básico, sin utilizar los giroscopios de la extensión del wiimote plus.

Dentro de la escena se apunta hacia los obstacúlos que se quieran disparar, usando la barra del sensor de la wii, y se presiona el botón A en el wiimote para disparar.

## Próximamente

* Uso del wiimote con la extensión plus.
* Uso de mas controladores, por determinar.

## Líbrerias usadas

Para la parte del wiimote se usa la libreria hecha por Adrian Biagioli disponible en el siguiente link:

https://github.com/Flafla2/Unity-Wiimote/
