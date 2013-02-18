Sample code for rendering a Geometry Wars style grid with the simulation done on the GPU.

The folder organization is different than in the actual source code, it's grouped here this way to make it clearer.




The Diagrams folder contains diagrams and an explanation of how the effect works overall.

Spring_hlsl coontains the shader hlsl source code

Free List Stroring some code related to "recycling"  killed objects instead of invoking the new operator.

Instancing some generic code used in instancing draw calls

Spring_Post_Process the "main" class of the effect used to sequence the right GPU draw calls, as well as the class acting as an interface for gameplay code to place forces in the grid.

The Bullet_Lib folder contains undocumented F# source code that is supposed to be a small dsl language for creating bullet patterns. I am not sure if it can ever be used in realtime and it's unfinished but I placed it here to show I can adapt to different paradigms and styles of programming.


The Polygon_Test folder contains a VS 2010 XNA project for a "tool" that allows you to draw a counterclockwise polygon, and then subdivides it in a data structure for facilitating a log(number_of_vertices) query on the polygon's most extreme vertices in a given direction. This will later be used in implementing the GJK algorithm for checking polygon interesction (currently I only use obbs spheres and planes).

(
Instructions: Draw a simple convex polygon by clicking the mouse, close it by clicking at the beginning, then press the left trigger to choose a direction and the A and Y buttons to rotate the polygon
TODO: Check convexivity/decompose to convex parts, run the GJK on 2 polygons 
moved by the triggers, get this to use a mesh output by Blender3D or other modelling programs
)