This diagram shows the sequence of effects used in the GPU spring simulation.

On the left column the rectangles are Direct3D effects, and they get called sequentially from  top to bottom.

To the left of those rectangles theres an abbreviation of the vertex format used in the draw call


S_D_F_V_F
Spring Draw Force Vertex Format

S_P_V_F
Spring Processing Vertex Format

F_D_I
Force Draw Instance

( 
...
	//this specifies the characteristics of the force, it's used in instancing along with a mesh describing the shape of the area where the forces are placed 
	Force_Draw_Instance 
	

	//The vertex format used when drawing line sprites based on the positions of the point masses of the grid
	Spring_Drawing_Vertex_Format

	//The vertex format used when drawing a full screen quad asking the GPU to run steps of  the simulation
	Spring_Processing_Vertex_Format

)


The right column contains the used Surfaces. To its right there is also the the pixel type of the surface.

The arrows indicate whether an effect reads a surface as a texture (the arrow is incoming to the effect's rectangle) or writes to the surface using it as a render-target.

The write operation arrows also have an A or O on them to indicate the blending mode (Additive Opaque etc.)


The Spring_Draw_Force_Vertex_Format currently contains the same type of data as the Spring_Processing_Vertex_Format though it has a different name just in case.
It is used to describe the "mesh" vertex stream when instancing the drawing of force quads.