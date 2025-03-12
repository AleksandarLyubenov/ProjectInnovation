Shader "Custom/CircleShader"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0,5)) = 1
        _Center ("Center", Vector) = (0,0,0,0)
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };
            
            fixed4 _MainColor;
            float _Radius;
            float4 _Center;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.worldPos, _Center);
                float alpha = smoothstep(_Radius, _Radius-0.1, dist);
                return fixed4(_MainColor.rgb, alpha * _MainColor.a);
            }
            ENDCG
        }
    }
}