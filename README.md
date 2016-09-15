# New Features
25/08/2016 [Feature request] Color spaces. ColorBand now supports HSV input. Documentation to be updated soon.

15/08/2016 ColorBand now supports discretization which subdivides the ColorBand into constant intervals.

# ColorBands
Unity 3D's *Gradient* data type allows you to set maximum 8 color keys. *ColorBand* data type has a different editor that doesn't suffer this limitation and is still easy to use. You can create your own ColorBands, store them and access them from code with an *Evaluate* method to get the color at time t, as for *Gradient*. RGB values are determined by curves, allowing a better control over how the color function evolves between your points.
Color bands are used in all kinds of applications in data visualization and other fields.

![Screenshot_01.PNG](https://github.com/rstecca/ColorBands/blob/master/Images/screenshot_01.png)

**Examples of color bands you cannot obtain with Unity's Gradient**

![You cannot do this with Unity 01](https://raw.githubusercontent.com/rstecca/ColorBands/master/Images/CannotDoThisWithGradients%2001.png)

![You cannot do this with Unity 02](https://raw.githubusercontent.com/rstecca/ColorBands/master/Images/CannotDoThisWithGradients%2002.png)

![You cannot do this with Unity
03](https://raw.githubusercontent.com/rstecca/ColorBands/master/Images/_CantDoThisWithGradients%2003.png)

## Usage
1) **Create a new Color Band asset**

![Screenshot_02.PNG](https://github.com/rstecca/ColorBands/blob/master/Images/Screenshot_02.png)

Newly created ColorBands will be placed in Assets folder's root.

2) **Change its name and its red, green, blue and alpha curves to obtain the desired effect**. You can hit *Set as filename* to quickly set the name from the new color band's filename. Note that curves' values should remain between 0 and 1 in both dimensions *time* and *value*.

![Screenshot_03.PNG](https://github.com/rstecca/ColorBands/blob/master/Images/Screenshot_03.png)

![Screenshot_05.PNG](https://github.com/rstecca/ColorBands/blob/master/Images/Screenshot_05.png)

3) **Declare a public ColorBand variable in your script**

4) **Assign a color band asset to it**

5) **Use it in code by calling the *ColorBand.Evaluate(float t)* method** where t is a floating point value between 0 and 1.

![Screenshot_04.PNG](https://github.com/rstecca/ColorBands/blob/master/Images/Screenshot_04.png)

## Discretization
A ColorBand can be discretized which means it will be turned into a set of flat intervals that will return a constant color.
To make a ColorBand discrete just set its discrete toggle to true and decide the number of steps the ColorBand will be subdivided into. This will result in discrete bands like the following:

![Discrete02.png](https://github.com/rstecca/ColorBands/blob/master/Images/Discrete02.png)

Three different discretization methods are available:
- LEFT_VALUE will build color intervals by evaluating the color at their left extreme.
- RIGHT_VALUE will build color intervals by evaluating the color at their right extreme.
- CENTER_VALUE will build color intervals by evaluating the color at their center.

## Color Space

![ColorCpace.png](https://github.com/rstecca/ColorBands/blob/master/Images/ColorSpace.png)

ColorBands can be described in the two main, standard color spaces RGB and HSV. By default a ColorBand will be set to RGB. When changing color space all the curves remain unvaried but they represent the respective values of the two spaces so that when switching to HSV, the first curve becomes hue, the second one saturation and the third one value. The alpha curve has the same meaning in both color spaces.

![HSVExample.png](https://github.com/rstecca/ColorBands/blob/master/Images/HSVExample.png)
