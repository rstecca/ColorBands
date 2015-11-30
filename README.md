# ColorBands
**Color band *data type* for Unity 3D**. This provides an easy way to create color bands in Unity Editor and access them from code with an evaluate method to get the color at time t. Color bands are used in all kinds of applications in data visualization and other fields.

![Screenshot_01.PNG](https://github.com/rstecca/ColorBands/blob/master/Images/screenshot_01.png)

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
