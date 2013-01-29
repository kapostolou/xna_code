// the surface containing the forces that each point mass should integrate this simulation step
// it is declared as half4 since it's an hdrblendable surface
Texture2D<half4> Force;
sampler forces = sampler_state { texture = <Force>; magfilter = POINT; minfilter = POINT; mipfilter=NONE; AddressU = clamp; AddressV = clamp;};

//the current velocity to be updated, the surface was the "front buffer" in the previous simulation step
Texture2D<float4> Old_Velocity;
sampler velocities = sampler_state { texture = <Old_Velocity>; magfilter = POINT; minfilter =POINT; mipfilter=NONE; AddressU = clamp; AddressV = clamp;};

//note that each texel in the above surfaces corresponds to a point mass etc., as commented elsewhere such as in 
//the Spring_Gather_Forces comments

#include "Parametrize.fxi"
//Included from Parametrize.fxi
///*** const float time_step;      //the sub-frame step's duration

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


float4 UpdateVelocities(float2 point_mass_texel_coord : TEXCOORD0) : COLOR0
{
  
    // the ps shader performs an euler method integration step
	
	float4 force=  tex2D(forces, point_mass_texel_coord);
	float4 velocity= tex2D(velocities,  point_mass_texel_coord);
	float4 result;
	result.xy=force.xy*time_step+0.99*velocity.xy;
	
    result.zw=float2(0,1);

    return result;
}


technique Velocity_Tech
{
    pass Pass1
    {
		VertexShader =  compile vs_3_0 VS();
        PixelShader = compile ps_3_0 UpdateVelocities();
    }
}