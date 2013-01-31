const float step=1/(128.0f);
const float half_step=1/(2*128.0f);


#include "Parametrize.fxi"

//The spring constant , drag coefficient, and another spring constant that has to do with an optionally used extra 
//spring to the point mass's original position, they are included form the Parametrize.fxi file which also contains
//other variables that seem can considerably tweak the end effect
//***     const float k=1200.0f;
//***     const float l=6.0f;
//***     const float k_equil=0;



//This texture doesn't get updated, it contains the initial positions of each point mass, so that some force sending it directly back there can be added
//its contents could also be directly calculated by the shader without accessing texture memory
Texture2D<float4> Initial_Position;
sampler initial_positions = sampler_state { texture = <Initial_Position>; magfilter = POINT; minfilter = POINT; mipfilter=NONE; AddressU = clamp; AddressV = clamp;};


//Where the point masses were located in the previous simulation step
//each texel corresponds to one point mass, a full screen quad gets drawn with coordinates that sent 1 point mass to each pixel shader
//the texture is called the old_position cause it comes form the surface updated at the current-1 simulation step
Texture2D<float4> Old_Position;
sampler positions = sampler_state { texture = <Old_Position>; magfilter = POINT; minfilter = POINT; mipfilter=NONE; AddressU = clamp; AddressV = clamp;};

//the velocity of the point masses in the previous simulation step
Texture2D<float4> Old_Velocity;
sampler velocities = sampler_state { texture = <Old_Velocity>; magfilter = POINT; minfilter =POINT; mipfilter=NONE; AddressU = clamp; AddressV = clamp;};

struct VS_In{
float4 full_screen_quad_coordinates:POSITION ;
float2 point_mass_texel_coord: TEXCOORD0;
 
 };

struct VS_Out{
float4 full_screen_quad_coordinates:POSITION ;
float2 point_mass_texel_coord: TEXCOORD0;
 
 };

 

VS_Out VS(VS_In input) 
{
    
	VS_Out Output = (VS_Out)0;		
	
	Output.full_screen_quad_coordinates.xyz=input.full_screen_quad_coordinates.xyz;
	Output.full_screen_quad_coordinates.w=1;
	Output.point_mass_texel_coord=input.point_mass_texel_coord;
	
	return  Output;
}




float4 CalculateForces(float2 point_mass_texel_coord : TEXCOORD0) : COLOR0
{
 
    //We will use the tx coord "index" as supplied to the pixel shader via interpolation
	//to find the neighbouring point masses, the step is the difference in tx coords between each neigbouring texel (and via a bijection each point mass)
	float2 mass_point_texel_coord= point_mass_texel_coord;
	float2 up_mass_point_texel_coord= point_mass_texel_coord+ float2(0,-step);
	float2 down_mass_point_texel_coord=point_mass_texel_coord+ float2(0,step);
	float2 left_mass_point_texel_coord= point_mass_texel_coord+ float2(-step,0);
	float2 right_mass_point_texel_coord= point_mass_texel_coord+ float2(step,0);

	
	//Using the calculated texel indices the pixel shader reads the positions of each point mass
	//from a texture containing the current position of all point masses/texels

	float4 up_mass_point_position=  tex2D(positions, up_mass_point_texel_coord);
	float4 down_mass_point_position= tex2D(positions, down_mass_point_texel_coord);
	float4 left_mass_point_position= tex2D(positions, left_mass_point_texel_coord);
	float4 right_mass_point_position= tex2D(positions, right_mass_point_texel_coord);
	float4 position= tex2D(positions, mass_point_texel_coord);
	

	//Read the velocity of the point mass
    float4 velocity= tex2D(velocities, mass_point_texel_coord);
	
	//The vectors to the current positions of the neighbouring (with a spring attached) point masses
	
	float2 up_displacement_to_point_mass=up_mass_point_position.xy-position.xy;
	float2 down_displacement_to_point_mass=down_mass_point_position.xy-position.xy;
	float2 left_displacement_to_point_mass=left_mass_point_position.xy-position.xy;
	float2 right_displacement_to_point_mass=right_mass_point_position.xy-position.xy;
	

	//Find the lengths of the vectors to the neighbouring spring point masses
	
	float up_distance_to_point_mass=length(up_displacement_to_point_mass);
	float down_distance_to_point_mass=length(down_displacement_to_point_mass);
	float left_distance_to_point_mass=length(left_displacement_to_point_mass);
	float right_distance_to_point_mass=length(right_displacement_to_point_mass);
	

	//this is the hooke's law calculation, each mass point will be influenced by 4 hooke forces
	
	
	float4 force=float4(0,0,0,1);
	
	force.xy=
	 k*     up_distance_to_point_mass      *      normalize(up_displacement_to_point_mass.xy)
	+k*     down_distance_to_point_mass    *      normalize(down_displacement_to_point_mass.xy)
	+k*     left_distance_to_point_mass    *      normalize(left_displacement_to_point_mass.xy)
	+k*     right_distance_to_point_mass   *      normalize(right_displacement_to_point_mass.xy)
	-l*velocity.xy;
	
	//Now add an extra hooke force to a point permanently stuck to the point mass's original position

	float4 back_home=float4(0,0,0,0);
	back_home.xy=tex2D(initial_positions, mass_point_texel_coord);               
	float distance_from_home=length(back_home.xy-position.xy);
	float2 force_towards_original_position=normalize(back_home.xy-position.xy)*distance_from_home*(k_equil);
	force.xy+=force_towards_original_position;
	
	force.zw=float2(0,1);
	
	//The force gets written on the Vector4 texels of the render surface
	//this can be optimized to use Vector2 texels 
    return force;
	
}

technique Force_Tech
{
    pass Pass1
    {
	    VertexShader =  compile vs_3_0 VS();
        PixelShader = compile ps_3_0 CalculateForces();
    }
}
