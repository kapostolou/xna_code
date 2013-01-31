float4x4 ViewProjection;

#include "Parametrize.fxi"
// Included from Parametrize.fxi, the width of the drawn line sprites
///***      const float obb_width;

Texture2D<float4> Position;
sampler positions = sampler_state { texture = <Position>; magfilter = POINT; minfilter = POINT; mipfilter=NONE; AddressU = clamp; AddressV = clamp;};
struct VS_In{


//These two identify the "spring" (pair of point masses) we are drawing
float2 current_point_mass_texel_coords:TEXCOORD1;
float2 other_point_mass_texel_coords:TEXCOORD2;

//Which corner of the spring's quad the vertexshader is processing
float2 quad_vertex_indicator: TEXCOORD0;


 };


struct VS_Out{
float4 Position:POSITION;
float length:TEXCOORD3;
 };






VS_Out
 Draw_Spring_Grid_VS
 (VS_In input) 
{

    

    float2 ended_up=tex2Dlod
	(positions ,	 float4(input.current_point_mass_texel_coords.x,	 input.current_point_mass_texel_coords.y, 0.0, 0.0)	);
	
	float2 other_ended_up=tex2Dlod
	(positions ,	 float4(input.other_point_mass_texel_coords.x, input.other_point_mass_texel_coords.y, 0.0, 0.0)  	);

	float2 forward=other_ended_up.xy-ended_up.xy;
	float length_=length(forward);
	
	float2 forward_normal=normalize(forward);
	
	float2 side_normal=float2(-forward_normal.y, forward_normal.x);
	side_normal*=obb_width;
    VS_Out Output = (VS_Out)0;		
	Output.length=length_;
	
	//The if statement's clause checks if the currently processed vertex is the tl, br, bl, or tr vertex of the quad
	if( input.quad_vertex_indicator.y>=0.5f)
	{
	Output.Position.xy=other_ended_up.xy;
	}
	else
	{
	Output.Position.xy=ended_up.xy;
	}
	
	if( input.quad_vertex_indicator.x>=0.5f)
	{
	Output.Position.xy-=side_normal;
	}
	else
	{
	Output.Position.xy+=side_normal;
	
	}
	Output.Position.x*=50;//*(saturate((Output.Position.x+1)*0.5)*2-1);	
	Output.Position.y*=50;//*(saturate((Output.Position.y+1)*0.5)*2-1);	
	Output.Position.zw=float2(-0,1);
	Output.Position=mul(Output.Position,ViewProjection);
	return  Output;
}



float4 Draw_Spring_Grid_PS (VS_Out input) : COLOR0
{
	
	// This makes big lines slightly more bright
	// it's not of much use but I will refactor it to be another parametrization parameter

	float coeff=saturate(input.length  /  (20*1/128.0f));
	float3 ret=float3(0.3f,0.02f,0.02)*lerp(1.0f,0.8f,coeff);
	return float4(ret.x,ret.y,ret.z,0.6f);
}

technique Accumulator_Tech
{
    pass Pass1
    {


	    VertexShader =  compile vs_3_0 Draw_Spring_Grid_VS();
        PixelShader = compile ps_3_0 Draw_Spring_Grid_PS();
    }
}