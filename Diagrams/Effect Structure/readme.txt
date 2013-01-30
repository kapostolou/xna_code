In this (vague) diagram rectangles indicate classes and the ellipses data structures.


Dotted arrows represent some type system related association (inheritance, interface implementation, typing of data structures)



Squared arrows the use of type parameters in a generic template


Circled arrows a run time call from the game code



The unnamed ellipse  represents a dynamic array and it and the type of its elements (Force_Draw_Handle) commands for placing forces on the grid.



Essentialy the game play code should just add to this list Force_Draw_Handles containing information on what we'd like to place.




The draw loop has a list of objects implementing the Post_Process interface and calls them in turn, one of them is supposed to guide the GPU simulation of the spring grid.







=====================


The Spring_Effect class implements the Post_Process interface and drives the related gpu draw calls when asked to do so. It also manages surface initialization etc



The Force_Draw_Handle is a request for placing forces



The Spring_Grid_Force_Placement manager gathers requests for the placement of forces on the grid (Force_Draw_handles). 



The Generic_Instance_Manager class has generic code for instancing draw calls



And the Force_Draw_Instance (it's named just Force_Instance in the diagram) is a vertex format that contains info used in instancing 

=====================


Also the Force_Quad_Instances_Manager is essentially just an alias for the generic template with the type parametes fixed. It should have been named the Force_Mesh_Instances_Manager but currently only Quads are used.






So broadly speaking the Spring_Effect guides the draw calls and the use of vertex buffers etc. And the game code is supposed to communicate (place forces) with the Spring_Grid_Force_Placement.
The other classes support those operations.
