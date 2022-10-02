Shader "Unlit/test"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 pos : TEXCOORDS1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pos = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 brown = fixed4(0.83, 0.5, 0.3, 1.);
                fixed4 red = fixed4(1., 0., 0., 1.);
                fixed4 yellow = fixed4(1., 1., 0., 1.);
                float d = length(i.pos.xyz);
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                float offset = .01 * (sin(i.pos.x * 15.) + sin(i.pos.y * 19.) + sin(i.pos.z * 13.));
                fixed4 lava_col = fixed4(lerp(red, yellow, (d + offset - .85) * 3.));
                return lerp(lava_col, brown, smoothstep(0.95, 0.951, d+offset));
            }
            ENDCG
        }
    }
}
