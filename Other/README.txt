These classes are all to be changed and not directly related to how the effect works, so I don't see why I'd document them in detail



Draw_Forces is just a place to initialize a vertexbuffer and an indexbuffer for creating a "model" of the shape of the force placed on the grid. Normally this would be done via loading and possibly pre processing a file from some 3d modeling program,
however I so far only use meshes created manually.



These Vertex Buffers will be referenced by a MY_Model object, which is a temporary class that is supposed to represent models in the game. That object is initialized
by accessing the static properties of Draw_Forces  (this is not really the correct way to do this but I will change it)



Then the MY_Object multiton has all the initialized "models"



And the Slot_Info_Multiton initializes objects containing information on "what model to draw for each slot of an instancing scheme", these get passed to the constructor of the
Generic Instancing template.



Here the Slot Info Multiton only has 2 info objects, one used in instancing the drawing of  distortion quads and one for force quads.


This all relies too much on static initialization and in general won't stay like this but I was trying to get the effect to work fast  (a prototype) and be flexible to change so that I could experiment
