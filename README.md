TFG-Unity
==

Este proyecto tiene como objetivo estudiar diferentes controladores, como el Wiimote, en Unity para demostrar las ventajas y desventajas frente a los controladores tradicionales.

Para esto, se realizarán distintas escenas en Unity demostrando las capacidades de los controladores usando librerías propias o encontradas por github.

## Contenido del proyecto

A continuación, se explicará la conexión de los diferentes controladores, conforme los vaya haciendo, y las distintas escenas que los usen.

## Conexión Wiimote

Una vez realizado cada método debería de detectarse automáticamente en Play el primer Wiimote conectado en las escenas de Unity.

### Barra sensor Wii

Para poder hacer uso de la barra de sensor infrarroja de la Wii en el PC se puede fabricar una barra casera o simplemente encender la Wii y situar la barra de sensor que tengas conectada encima de la pantalla del pc.

### Windows

1. Pulsa el botón derecho del ratón en el icono de bluetooth de la barra de notificaciones.
2. Clicar unirse a una red de área personal.
3. Pulsando el botón de agregar dispositivo te saldrá una ventana que buscará automáticamente los dispositivos.
4. Mientras busca pulsa 1+2 en el wiimote hasta que aparezca, puede ser necesario más de una búsqueda.
5. Seleccionar el wiimote en la ventana de búsqueda y dejar el pin vacío.

PD: Este método requiere desvincular el Wiimote del pc y reconectarlo cada vez que se reinicie el PC.

Source: Experiencia y distintas fuentes.

### Linux

1. Instala cwiid con el siguiente comando de bash.
> sudo apt-get install libcwiid1 lswm wmgui wminput

2. Arranca el programa con el siguiente comando de bash.
> wmgui

3. Selecciona "connect" en el menú de archivo y pulsa 1+2 en el Wiimote cuando te diga el programa.

Source: https://askubuntu.com/questions/705865/how-to-pair-wiimote-in-ubuntu-15-10

PD: Método no probado aún.

## WiimotePlayDemo

En esta escena solo se utiliza el hardware del Wiimote básico, sin utilizar los giroscopios de la extensión del Wiimote plus.

Dentro de la escena se apunta hacia los obstáculos que se quieran disparar, usando la barra del sensor de la Wii, y se presiona el botón A en el Wiimote para disparar.

## Próximamente

* Uso del Wiimote con la extensión plus.
* Uso de mas controladores, por determinar.

## Librerías usadas

Para la parte del Wiimote se usa la librería hecha por Adrian Biagioli disponible en el siguiente link:

https://github.com/Flafla2/Unity-Wiimote/
