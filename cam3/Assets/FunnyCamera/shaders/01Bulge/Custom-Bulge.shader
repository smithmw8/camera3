﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Custom-Bulge" {
    Properties {

        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
         _Scale("Scale", range(-1,1)) = 0.8
    }
    
    SubShader {

    //    Tags { "RenderType"="Opaque" }
     
         Tags { 
           "Queue"="Transparent" 
         "RenderType"="Transparent" 
         }
        LOD 200
          
        Pass
        {
        	Blend SrcAlpha OneMinusSrcAlpha 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
              
            sampler2D _MainTex;
              fixed4 _Color;
            float  _Scale;
            float4 _MainTex_ST;
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
  
            v2f vert(appdata_full v)
            {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
            }
            
            float4 frag(v2f i) : COLOR
            {      
                float2 tc = i.uv;
                tc -= float2(0.5,0.5);
                float dist = length(tc);
                float radius = 0.5f;
                
                tc = i.uv;
                if (dist < radius)
                {
                	tc -= float2(0.5,0.5);
                    float percent = 1.0 - ((radius - dist) / radius) * _Scale;
                    percent = percent * percent;
                    
                    tc = tc * percent;
                    tc += float2(0.5,0.5);
                }
                float3 color = tex2D(_MainTex, tc).rgb;
                return float4(color, 1.0)* _Color;
            }
              
            ENDCG
        }
    }
    FallBack "Diffuse"
}