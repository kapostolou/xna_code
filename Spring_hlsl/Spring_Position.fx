
#include "Parametrize.fxi"
//Included from Parametrize.fxi
///*** const float time_step;      //the sub-frame step's duration



// a simple euler method integration step


Texture2D<float4> Velocity;
sampler velocities = sampler_state { texture = <Velocity>; magfilter = POINT; minfilter =POINT; mipfilter=NONE; AddressU = clamp; AddressV = clamp;};

Texture2D<float4> Old_Position;
sampler positions= sampler_state { texture = <Old_Position>; magfilter = POINT; minfilter = POINT; mipfilter=NONE; AddressU = clamp; AddressV = clamp;};


// as decribed in the Spring_Gather_Forces.fx file the point_mass_texel_coord is a txcoord xorresponding to a point mass in the grid
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


float4 UpdatePositions(float2 point_mass_texel_coord : TEXCOORD0) : COLOR0
{
	
	float4 velocity=  tex2D(velocities, point_mass_texel_coord);
	
	float4 position=tex2D(positions, point_mass_texel_coord);

	float4 result=time_step*velocity;
	
	result.xy+=position.xy;
	
	
	
	result.zw=float2(0,1);
    return result;

	
}

technique Position_Tech
{
    pass Pass1
    {
	VertexShader =  compile vs_3_0 VS();
    PixelShader = compile ps_3_0 UpdatePositions();
    }
}
