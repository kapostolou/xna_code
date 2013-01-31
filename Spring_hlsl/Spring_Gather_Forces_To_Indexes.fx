#include "Helpers.fxi"

const float size=128.0f;
const float step=1/(128.0f);
const float half_step=1/(2*128.0f);

//This surface contains forces written on texels
//but spatially, that is each point on the surface represents a small part of the plane
//this shader is supposed to for each such part, apply its forces to all the point masses currently within it
//then once the forces have been "gathered" per mass point, the mass points will also get the hooke's law forces due to the springs
//the computation isn't structured "per area texel" though, each pixel shader gets a texel corresponding
//to a mass point (via a full screen quad) and then reads both the mass point's position and the Force_Spatial_Accumulator surface containing the geometrically placed forces

Texture2D Force_Spatial_Accumulator;
sampler spatially_placed_forces = sampler_state { texture = <Force_Spatial_Accumulator>; magfilter = POINT; minfilter = POINT; mipfilter=NONE; AddressU = clamp; AddressV = clamp;};


Texture2D<float4> Old_Position;
sampler positions = sampler_state { texture = <Old_Position>; magfilter = POINT; minfilter = POINT; mipfilter=NONE; AddressU = clamp; AddressV = clamp;};


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
	
	//copy the full screen quad's vertex data to the pixel shader
	Output.full_screen_quad_coordinates.xyz=input.full_screen_quad_coordinates.xyz;
	Output.full_screen_quad_coordinates.w=1;
	Output.point_mass_texel_coord=input.point_mass_texel_coord;
	
	return  Output;
}



//This pixel shader sends the forces to the mass points
//the -force per mass point surface-(where this pixel shader draws)
//is an hdrblendable surface so that it can be additively blended with in the shader that
//calculates the hooke forces

float4 Gather_Forces(VS_Out Input) : COLOR0
{
 

 float2 point_mass_texel_coord= Input.point_mass_texel_coord;
 
 //Find the current position of the point mass
 float4 where_it_is= tex2D(positions, point_mass_texel_coord);
 where_it_is.zw=float2(0,1);
 
 //Transform the position to the adressing scheme used by hlsl when sampling textures
 float2 where_it_is_UV=	Clip_Point_To_UV_Point( where_it_is);
 
 //where_it_is_UV.x=floor(where_it_is_UV.x*128.0f)/128.0f+1/256.0f;
 //where_it_is_UV.y=floor(where_it_is_UV.y*128.0f)/128.0f+1/256.0f;
 
 //Read the total forces placed on this area from the accumulator
 //removing the if statement gives a much wilder effect on the grid's edges
 //as due to the clamping in the texture access
 //all mass points outside the grid will get the forces placed on its edges
 //this could be enabled or not 
 //(the if sentence could be removed by setting the texture addressing to BORDER and the "color" to the zero 4-vector

 float4 read_force;
 if(abs(where_it_is_UV.x-0.5f)<0.5f&&abs(where_it_is_UV.y-0.5f)<0.5f)
 read_force=tex2D(spatially_placed_forces,where_it_is_UV.xy);
 else
 read_force=float4(0,0,0,0);
 return float4(read_force);
}

technique Tech
{
    pass Pass1
    {
	    VertexShader =  compile vs_3_0 VS();
        PixelShader = compile ps_3_0 Gather_Forces();
    }
}
