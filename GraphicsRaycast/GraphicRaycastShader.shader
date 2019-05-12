Shader "Hidden/Raycast" {
    SubShader 
    {
	Tags { "RenderType"="Opaque" }

    Pass
    {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D_float _CameraDepthTexture;

            struct v2f {
               float4 pos : SV_POSITION;
               float4 scrPos : TEXCOORD0;
               float3 normal : NORMAL;
            };

            //Vertex Shader
            v2f vert (appdata_base v){
               v2f o;
               o.pos = UnityObjectToClipPos (v.vertex);
               o.scrPos = ComputeScreenPos(o.pos);
               o.normal = v.normal;
               return o;
            }

            //Fragment Shader
            half4 frag (v2f i) : COLOR
            {
               float3 wNormal = UnityObjectToWorldNormal(i.normal.xyz);

			   //Remap from [-1 to 1] to [0 to 1]
			   wNormal.rb = wNormal.rb * 0.5 + 0.5;		
               
               float depth = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);

               return float4(wNormal, depth);

            }
        ENDCG
        }
    }
	FallBack "Diffuse"
}