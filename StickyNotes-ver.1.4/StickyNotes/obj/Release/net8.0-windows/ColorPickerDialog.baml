   M S B A M L   `   `   `  ����   StickyNotes#    StickyNotes.ColorPickerDialog   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  选择颜色��$	��300��$	��300�� � WindowStartupLocation$ CenterOwner=� � 
ResizeMode$ NoResize=� � SizeToContent$ Manual=�.��� $��10q�.+��� .+���  �� Orientation$ 
Horizontal=�.+��� $��	红色：��$��Center=��� -    ��Name$ 	RedSlider�� � Minimum$ 0Q� � Maximum$	 255Q�$	��200���� $ 
Horizontal=�.+��� $��	绿色：��$��Center=��� -   $ GreenSlider��$ 0Q�$	 255Q�$	��200���� $ 
Horizontal=�.+��� $��	蓝色：��$��Center=��� -   $ 
BlueSlider��$ 0Q�$	 255Q�$	��200���� $ 
Horizontal=�$
��0,10q�.+��� -   $��30��$��30��$��2q� � Cursor$
 Hand�����	#FFFF0000
	 � Tag$	 255,0,0��
���   �$��1q��� -   $��30��$��30��$��2q�$
 Hand�����	#FF00FF00$	 0,255,0��
���   �$��1q��� -   $��30��$��30��$��2q�$
 Hand�����	#FF0000FF$	 0,0,255��
���   �$��1q��� -   $��30��$��30��$��2q�$
 Hand�����	#FFFFFF00$	 	255,255,0��
���   �$��1q��� -   $��30��$��30��$��2q�$
 Hand��
�������$	 255, 255, 255��
���   �$��1q��� $��50��
�������$��1q����� *PreviewColor+�� $ 
Horizontal=�$��Right=�$
��0,10q�.+��� -	   $��确定��$��80���� -
   $��取消��$��80��$��5,0,0,0q�