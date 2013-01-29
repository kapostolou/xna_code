//Gets a stream of "force placing shapes"
//The inputs are coming from 2 vertex streams, one containing the "model" of the placed force
//that is how the area under which the texels that will get the force placed is shaped,
//the other stream containing details about this particular instance of the placement.

//Currently all the shapes are just unrotated quads but I'll try to see what it'll look like with different shapes.

//Unlike in the other hlsl effects used in running the GPU spring simulation
//here the quads will be placed on specific areas of the surface (not with a full screen quad sending texel adresses to pixel shaders)

//They'll also be drawn with an additive blend on an hdrblendable surface (added that is)

//The mesh vertex stream's position0 data contain the normalized coordinates position of an unrotated full screen quad (or in the future mesh in general)
//Using the halfaxis and force_center parameters the quad gets scaled and translated to some specific area (all transformations 
//are done directly in normalized coordinates)

//I will eventually change the "instance" that the first vertex stream contains
//to contain scaling and rotation matrices and do the calculation fully in the GPU (and support general shapes instead of unrotated quads)

// parameters
// force_center: is a normalized coordinates point where the "center" the force is supposed be emmited from
// it depends on the type of calculation whether it is needed or not

// direction: shows the way the object that placed the force was moving (currently unused)
// halfaxis: the radius of the nonrotated quad 
// force_strength: modulates how strong the force will be (it is supposed to be up to the particular calculation method if and how it will be used)


#include "Parametrize.fxi"
//****Constants taken from Parametrize.fxi
//***        const float scale_factor;                        //modifies tha area affected
//***        const float magn_factor;                         //modifies the forces magnitude
//***        const float minimum_allowed_point_distance;      //threshold for how small a distance can be in an inverse square calculation

struct VS_In
{
float4 position:POSITION;
float2 force_center:TEXCOORD1;
float2 direction:TEXCOORD2;
float  halfaxis:TEXCOORD3;
float  force_strength:TEXCOORD4;
};

struct VS_Out
{
float4 Position:POSITION ;
float2 Position_Copy: TEXCOORD0;
float2 force_center:TEXCOORD1;
float2 direction:TEXCOORD2;
float  force_strength:TEXCOORD3;
};

VS_Out Draw_Force_VS(VS_In input)
{
	VS_Out Output;
	//it is up to the pixel shader wether to use these arguments or not
	Output.force_center=input.force_center;
	Output.direction=input.direction;
	Output.force_strength=input.force_strength;
	
	//this scales and translates the quad
	float2 normalized_coord_position=input.position.xy*scale_factor*input.halfaxis+input.force_center;
	Output.Position.xy=normalized_coord_position.xy;
	
	//Tell the pixel shader the position it's working on so that it can be compared to the quads center
	Output.Position_Copy.xy=normalized_coord_position.xy;
	Output.Position.zw=float2(0.0f,1);
	
	
	return Output;
}


float4 Draw_Force_PS (VS_Out input) : COLOR0
{
  
  //vector from where the force is supposed to be emmited from to the texel the ps is working 
  float2 direction_towards_the_emission=-normalize(input.Position_Copy.xy-input.force_center);
  
  //the length ot the previous vector, it can be maxed by a threshold, clamping this to a small value
  //boosts the applied forces due to the division by the square length_*length_
  float length_=max(length(input.Position_Copy.xy-input.force_center),minimum_allowed_point_distance);
  
  //something vaguely similar to the gravitation force, the *14 coefficient is a remnant from back when the effect worked differently
  //and the scale of the forces was greater
  return float4 (magn_factor*14*input.force_strength*direction_towards_the_emission/(length_*length_),0,1);
  
 
 
  
}

 

technique Force_Draw_Tech
{
    pass Pass1
    {


		VertexShader =  compile vs_3_0 Draw_Force_VS();
        PixelShader = compile ps_3_0 Draw_Force_PS();
    }
}
