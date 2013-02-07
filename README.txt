Sample code for rendering a Geometry Wars style grid with the simulation done on the GPU.

The folder organization is different than in the actual source code, it's grouped here this way to make it clearer.




The Diagrams folder contains diagrams and an explanation of how the effect works overall.

Spring_hlsl coontains the shader hlsl source code

Free List Stroring some code related to "recycling"  killed objects instead of invoking the new operator.

Instancing some generic code used in instancing draw calls

Spring_Post_Process the "main" class of the effect used to sequence the right GPU draw calls, as well as the class acting as an interface for gameplay code to place forces in the grid.

The Bullet_Lib folder contains undocumented F# source code that is supposed to be a small dsl language for creating bullet patterns. I am not sure if it can ever be used in realtime and it's unfinished but I placed it here to show I can adapt to different paradigms and styles of programming.