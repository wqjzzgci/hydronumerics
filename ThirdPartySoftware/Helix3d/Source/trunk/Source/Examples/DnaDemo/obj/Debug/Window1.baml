   M S B A M L   `   `   `  ����Z  VHelixToolkit, Version=0.9.4175.40396, Culture=neutral, PublicKeyToken=52aa3500039caf0dA0clr-namespace:HelixToolkit;assembly=HelixToolkitHelixToolkit  5       DnaDemo   DnaDemo.Window1   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Bhelix0clr-namespace:HelixToolkit;assembly=HelixToolkit     5        � Title$  Double helix demo��5      $	��480��6   $	��640��6,   
���   � � 	Resources 5         RainbowBrush �� 
StartPoint �� EndPoint � Color � Offset��%�  &	        �� 5
   
   $	 0,0&�63   $	 1,0&�6D   ��5      � $	 Red��6   $ 0Q�6+   � 5      $ Orange��6   $
 0.17Q�6.   � 5      $ Yellow��6   $
 0.33Q�6.   � 5      $ Green��6   $	 0.5Q�6-   � 5      $
 Blue��6   $
 0.67Q�6,   � 5      $ Indigo��6   $
 0.84Q�6.   � 5      $ Violet��6   $ 1Q�6.   5      5      .��5      � .+�5   
      HelixToolkit.HelixView3D  -   5          Name$ view1��5         ShowFrameRate .    InfiniteSpin . 	   IsHeadLightEnabled	 . 
   CameraRotationMode$
 VirtualTrackball=�5         Children. 5      g� -   5        	model�� g� 	Transform 5      S� 5       S� OffsetZ$	 -15Q�6+   .2�5"          HelixToolkit.HelixVisual3D     Radius$	 0.4Q�6&      Turns$ 3Q�63      Length$ 30Q�6=       HelixToolkit.MeshElement3D   Fill# [  6I     5#      $	 0.4Q�6&   $ 3Q�63   $ 30Q�6=      Phase$
 3.14Q�6I   # [  6V   5%      5'      5(      5)      