# ColorBands
Unity 3D's *Gradient* data type allows you to set maximum 8 color keys. *ColorBand* data type has a different editor that doesn't suffer this limitation and is still easy to use. You can create your own ColorBands, store them and access them from code with an *Evaluate* method to get the color at time t, as for *Gradient*. RGB values are determined by curves, allowing a better control over how the color function evolves between your points.
Color bands are used in all kinds of applications in data visualization and other fields.
No support for alpha channels **yet**.

![Screenshot_01.PNG](https://github.com/rstecca/ColorBands/blob/master/Images/screenshot_01.png)

**Examples of color bands you cannot obtain with Unity's Gradient**

![You cannot do this with Unity 01](https://raw.githubusercontent.com/rstecca/ColorBands/master/Images/CannotDoThisWithGradients%2001.png)

![You cannot do this with Unity 02](https://raw.githubusercontent.com/rstecca/ColorBands/master/Images/CannotDoThisWithGradients%2002.png)


## Usage
1) **Create a new Color Band asset**

![Screenshot_02.PNG](https://github.com/rstecca/ColorBands/blob/master/Images/Screenshot_02.png)

2) **Change its name and its red, green and blue color curves to obtain the desired effect**

![Screenshot_03.PNG](https://github.com/rstecca/ColorBands/blob/master/Images/Screenshot_03.png)

![Screenshot_05.PNG](https://github.com/rstecca/ColorBands/blob/master/Images/Screenshot_05.png)

3) **Declare a public ColorBand variable in your script**

4) **Assign a color band asset to it**

5) **Use it in code by calling the *ColorBand.Evaluate(float t)* method** where t is a floating point value between 0 and 1.

![Screenshot_04.PNG](https://github.com/rstecca/ColorBands/blob/master/Images/Screenshot_04.png)
