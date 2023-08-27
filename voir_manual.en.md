# Manual of VOIR
## About VOIR
VOIR is an interactive visualization software for HMD-type VR devices. It is developed upon Unity.
This software provides an interactive visualization environment equivalent to VFIVE, a visualization software for the CAVE systems, even though it can be executed on a PC and HMD.
VOIR can visualize three-dimensional scalar and vector data defined in the Cartesian coordinates.
It is also possible to display surfaces in STL format(binary file) and curves to display, for example, the boundaries.
The data required to start VOIR are
1. scalar data (up to 3)
2. vector data (up to 3)
3. coordinate data (for Rectilinear)
4. surface data (STL format, optional)
5. line data (optional)
This information must be described in the VOIR configuration file (JV file).
The JV file is explained later.
At least one scalar data or one vector data are required.
The maximum number of scalar and vector data are specified in a file named voirConst.cs as
> public const int MAXNVECT = 3;<br>
> public const int MAXNSCAL = 3;
## Data format
### Scalar and Vector data
Scalar and Vector data have to be prepared as binary data (ex. Fortran’s unformatted files) of single or double-precision real numbers, where SIZEX is the grid size in the X direction, SIZEY in the Y direction, and SIZEZ in the Z direction,
- C: double(or float) data[SIZEZ][SIZEY][SIZEX]
- Fortran: real*8(or real) data(SIZEX, SIZEY, SIZEZ)
The X, Y, and Z components have to be prepared separately for vector data.
### Coordinates data (for rectilinear)
The coordinate data for the x, y, and z directions must be prepared as double-precision floating-point binary data.
- C : double x[SIZEX], double y[SIZEY], double z[SIZEZ]
- Fortran: real*8 x(SIZEX), y(SIZEY), z(SIZEZ)
## Configuration file (JV file)
To use VOIR, users have to prepare a configuration file called JV file (<u>J</u>e <u>V</u>ois). An example is shown below. Lines beginning with ‘#’ are parsed as comments. Keywords and values are separated with spaces.
    # VOIR Config file (example.jv)
    #
    # Grid size in x, y and z direction
    GRIDSIZE   200 200 200
    # Number of vector data
    NVEC    1
    # Number of scalar data
    NSCAL   2
    #--------- for equally spaced grids
    UNIFORM
    # Grid spacing in the x, y and z directions
    DX 0.005 0.005 0.005
    # Grid corner positions of the x, y and z directions
    CORNER -0.5 -0.5 -0.5
    #--------- for Rectilinear, Prepare coordinate files for the x, y, and z directions separately
    #COORDFILES   c:\Users\ohno\dynamo\dynamo.x c:\Users\ohno\dynamo\dynamo.y c:\Users\ohno\dynamo\dynamo.z
    # when a 4-byte header/footer is added to data files or coordinate files (for example, Fortran’s unformatted files)
    SKIP4BYTES
    # For single-precision data files
    SINGLEPRECISION
    # Label of the first vector data (for menu). ‘@s’ signifies a space.
    VECT0_LABEL  Velocity@sField
    # x component of the first vector data file
    VECT0X  c:\Users\ohno\dynamo\dynamo.vel.xf
    # y component of the first vector data file
    VECT0Y  c:\Users\ohno\dynamo\dynamo.vel.yf
    # z component of the first vector data file
    VECT0Z  c:\Users\ohno\dynamo\dynamo.vel.zf
    # Label of the first scalar data (for menu). ‘@s’ signifies a space.
    SCAL0_LABEL vorticity_z
    # The first scalar data file
    SCAL0   c:\Users\ohno\dynamo\dynamo.vorticity_zf
    # Label of the second scalar data (for menu). ‘@s’ signifies a space.
    SCAL1_LABEL vor_z_sq
    # The second scalar data file
    SCAL1   c:\Users\ohno\dynamo\dynamo.vor_z_sqf
    # Curve file
    CURVEFILES c:\Users\ohno\dynamo\cindex.c c:\Users\ohno\dynamo\linex.c c:\Users\ohno\dynamo\liney.c c:\Users\ohno\dynamo\linez.c
    # Surface file (STL format)
    SURFACEFILES  c:\Users\ohno\dynamo\surfstl.stl
## Excution of VOIR
To run VOIR, enter the following line (with the JV file as an argument) at the command prompt,
    > voir example.jv
When one runs VOIR on Unity Editor,
the JV file must be specified in the following way in voirConst.cs
> public const string paramfile = “Assets/data/example.jv”;
When the program is executed, red (x-direction), green (y-direction), and blue (z-direction) lines will appear, indicating the boundaries of the data.
## Operation
One uses both the left and right controllers.
Select a data label to visualize from the menu and then select a visualization method.
### Joystick
The joystick on the right controller is used to move in the VR space. Point the right controller in the direction to move and tilt the joystick forward (backward) to move forward (backward) in the direction. Tilt the joystick left or right to rotate.
### Buttons
The left and right controller trigger and grip buttons are used.
The trigger button on the right controller is used to select menu items and change visualization parameters, while the grip button is used to erase visualized objects. The trigger button on the left controller is used to turn menus on and off, and the grip button is used to erase all streamlines.
### Menu
The menu appears when the trigger button on the left controller is pressed.
Select an item by aiming it with the red laser pointer emitted from the right controller and pressing the trigger button on the right controller.
If there are submenus, they are displayed on the right one after the other.
For example, to perform isosurface visualization, select “Scalar Field” menu. Then a list of scalar data is shown as a submenu. Select a panel in the list to apply the isosurface visualization. Then another submenu appears in the right-side in which visualization methods that can be applied to the scalar data are shown. Among them, select “Isosurface” panel. Then another submenu appears by which one can choose wireframe or fully rendered surface renderings. Finally, one can control the coloring scheme of the isosurface by yet another sub menu.
### Visualization Methods
| Visualization Methods | Usage |
|:---:|---|
|Isosurface<br>(Scalar Data)|The isosurface level can be changed by holding down the trigger button on the right controller and moving the controller up or down. Releasing the button determines the level and the isosurface is recalculated. It is possible to display isosurfaces in the wireframe and to color them through transfer functions.|
|Ortho Slices<br>(Scalar Data)|The data can be sliced in the XY, YZ, and XZ planes. The position of the slice can be changed by holding down the trigger button on the right controller and moving the controller up, down, left, right, forward, or backward. Releasing the button determines the position of the slice, and the slicing plane is recalculated. It is also possible to display the slice in the wireframe and with bumps.|
|Local Slice<br>(Scalar Data)|When the trigger button on the right controller is pressed, a laser is emitted from the right controller, and the local slice surface is displayed at the tip of it. By holding down the button and moving the right controller, the local slice can be moved to the position the user wants to locally slice the data.|
|Field Lines<br>(Vector Data)|When the trigger button on the right controller is pressed, a laser is emitted from the right controller. Move the controller with the button pressed to align the tip of the laser with the position where one wants to put a seed of a field line, and release the trigger button to start the field line calculation. The grip button on the left controller is used to erase the most recent field line, and the grip button on the right controller is used to erase all field lines.|
|Local Arrows<br>(Vector Data)|When the trigger button on the right controller is pressed, a laser is emitted, and a group of arrows appears around the tip of the laser. The position of the arrow group can be moved by holding down the trigger button and moving the right controller.|
|Flash Light<br>(Vector Data)|Pressing the trigger button on the right controller let you see a myriad of particles move along the vector field within a cone-shaped area in front of the right controller (as if illuminated by a flashlight). Moving the controller while holding down the trigger button moves the cone region.|
|Hotaru(Fireflies))<br>(Vector Data)|Numerous particles move along the vector field in the data. They look like lots of fireflies.|
Other Functions
| Functions | Usage |
|:---:|---|
|Snapshot|This function saves the displayed image as an image file. When the trigger button on the right controller is pressed, the displayed image is saved as a PNG format image file.|
|ROI|This function allows one to select a volume to be analyzed in more detail. Press the trigger button on the right controller to set the endpoint (starting point) of the volume to be selected, then move the controller while holding it down and release the trigger button to set the other endpoint (ending point). The cuboid surrounded by the two endpoints is the new analysis volume.|
|Objects|To toggle the visibility of specified surface/line objects. |
|QUIT| To terminate VOIR.|
### Format of curve files
Four files are required to display curves. All of them should be prepared as binary files (ex. Fortran unformatted file).
The first file contains the total number of curves and the numbers of vertices for each curve. They are stored as 4-byte integers. Suppose there are 3 lines, and the numbers of vertices for them are 10, 15, and 14, respectively. The content of this file is
    3 10 15 14
The x, y, and z coordinates of each vertex are recorded in the remaining three files in single-precision floating-point numbers. For example, the vertices are (3.0, 5.2, 2.1), (3.5, 5.5, 2.3), ... then
    File of x-coordinates: 3.0 3.5 ...
    File of y-coordinates: 5.2 5.5 ...
    File of z-coordinates: 2.1 2.3 ...