In this (vague) diagram rectangles indicate classes and the ellipses data structures.

Dotted arrows represent some type system related association (inheritance, interface imlementation, typing of data structures)

Squared arrows the use of type parameters in a generic template

Circled arrows a run time call from the game code

The unnamed ellipse  represents a dynamic array and it and the type of its elements (Force_Draw_Handle) are the most important from the point of view of the gameplay code.

Essentialy the game play code should just add to this list Force_Draw_Handles containing information on what he'd like to place to the grid as a force.

The draw loop has a list of objects implementing the Post_Process interface and calls them in turn, one of them is supposed to guide the GPU simulation
of the spring grid. The Spring_Effect class implements the interface and drives the related gpu draw calls when asked to do so.


The Spring_Effect manages the GPU calls/surfaces etc

The Spring_Grid_Force_Placement manager gathers
requests for placement os forces on the grid

The Generic Instance  class enables instancing

The Draw_Handle is a request for placing forces

And the Force_Draw_Instance (it's named just Force_Instance in the diagram) is a vertex format that contains info used in instancing 

The Force_Quad_Instances_Manager is essentially just an alias for the generic template with the type parametes fixed