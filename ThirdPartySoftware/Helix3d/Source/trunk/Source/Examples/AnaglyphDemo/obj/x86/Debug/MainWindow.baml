   M S B A M L   `   `   `  ����Z  VHelixToolkit, Version=0.9.4175.40396, Culture=neutral, PublicKeyToken=52aa3500039caf0dA0clr-namespace:HelixToolkit;assembly=HelixToolkitHelixToolkit  5   @   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 6{   G CAnaglyphDemo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:AnaglyphDemoAnaglyphDemo 6�      AnaglyphDemo.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   >h0clr-namespace:HelixToolkit;assembly=HelixToolkit     6@   9System&clr-namespace:System;assembly=mscorlib     6{   ,localclr-namespace:AnaglyphDemo     6�     � Title$  AnaglyphDemo��6�   $	��480��6�   $	��640��6�    � 	Resources 5       S� 
MethodName S� 
ObjectType  System.Enum   methods S� MethodParameters M� TypeName 
 brush1 j� ViewportUnits j� Viewport j� Viewbox	 j� ViewboxUnits
 L� Drawing � Brush � Geometry �� Rect  SemiSubmersible   HelixToolkit.BoxVisual3D   Width   Height   Length   Center    HelixToolkit.MeshElement3D   Fill(   "HelixToolkit.TruncatedConeVisual3D   
BaseRadius   	TopRadius   BaseCap   Origin   Height$   HelixToolkit.GridLinesVisual3D��%U  &	        &	 m     &	 �    S� 5   
   $ 	GetValues��6   "  64    5      M� 5      $ h:AnaglyphMethod��6   5	      L� 5
   
   $ Absolute=�6&   $ 0,0,0.1,0.1�6?   $ 0,0,1,1�6V   $	 Absolute=�6h   
 5      I� 5      .��5      � 
 ����� 5      �� 5      $ 0,0,1,1�60   5      � 5      
 �   � 5      �� 5      $ 0.25,0.25,0.5,0.5�60   5      5      5      g� 5   
   .2�5        $ 50Q�6   $ 4Q�6'   $ 50Q�62   $ 0,0,121�6>   
 �����6N     5      $ 6Q�6   $ 20Q�6&   $ 6Q�62   $ 0,0,241�6=   
 � ���6M     5      $ 10Q�6   $ 4Q�6'   $ 30Q�62   $ 0,10,161�6>   
 �����6O     5      $ 10Q�6   $ 4Q�6'   $ 10Q�62   $ 10,10,201�6>   
 �����6P     5      $ 5Q�6&   $ 5Q�65    . $ -20,-20,-101�6R   $ 20Q�6g   
 � � �6s     5       $ 5Q�6&   $ 5Q�65    . $ 
-20,20,-101�6R   $ 20Q�6f   
 � � �6r     5!      $ 5Q�6&   $ 5Q�65    . $ 
20,-20,-101�6R   $ 20Q�6f   
 � � �6r     5"      $ 5Q�6&   $ 5Q�65    . $ 	20,20,-101�6R   $ 20Q�6e   
 � � �6q     5#      $ -20,0,-61�6   $ 4Q�6.   $ 40Q�69   $ 4Q�6D   
 �����6O     5$      $ 20,0,-61�6   $ 4Q�6-   $ 40Q�68   $ 4Q�6C   
 �����6N     5%      $ 0,-20,-61�6   $ 4Q�6.   $ 4Q�69   $ 40Q�6C   
 �����6O     5&      $ 0,20,-61�6   $ 4Q�6-   $ 4Q�68   $ 40Q�6B   
 �����6N     5'      
 �����6"   5(      5)      .��5*      ]� .+�5+   
   o� $	��Top=�6   .@�5,      m� $
��File��6   5-      5.      �� 5/   
   $��Bottom=�6   .@�50      �� $��Model:��6   $��0 0 4 0q�6%   �� 51      -   5        $��model��51       �� DisplayMemberPath$ Title��6$   $	��100��6>   .@�52        AnaglyphDemo.ModelItem     Title$ Cubes��6"     53      $ Semi-submersible��6"      Model# [ 6;   54      �� 55      $��Method:��6   $��10 0 4 0q�6&   �� 56      -   5        $��method��56       �� SelectedIndex$ 5��6]   $	��100��6o   ��6%   ��  �� Source# [  6o   �� 57      $��Stereobase:��6   $��10 0 4 0q�6*   �� 58      -   5        $��
stereoBase��58       � Minimum$ 0Q�6'    � Maximum$ 5Q�63    � Value$
 0.15Q�6?   $	��100��6L   �� 59        �� Padding$  4 0 4 0q�6a   ��6   �� *Value+! �� ElementName$! 
stereoBase��" �� StringFormat$" 	{0:0.000}��6a   5:      !   HelixToolkit.AnaglyphView3D  5;   
   -   5        #  Name$# anaglyphView1��5;      
���   �    HelixToolkit.StereoControl$   CopyDirectionVector$ .  %   
StereoBase% 6E   �� *Value+$! 
stereoBase��&   Method& 6z   �� *SelectedValue+$! method��5<      5=      5>      