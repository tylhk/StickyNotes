   M S B A M L   `   `   `  ����  StickyNotes#    StickyNotes.ColorPickerDialog   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  选择颜色��5   	   $	��300��5   	   $	��300��5   	    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�6-    � SizeToContent$ Manual=�6C   .��5      � $��10q�6   .+�5	   
   �� .+�5
      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� $��	红色：��6   $��Center=�6'   �� 5      -   5         ��Name$ 	RedSlider��5       � Minimum$ 0Q�6,    � Maximum$	 255Q�68   $	��200��6F   6R   5      �� 5      $ 
Horizontal=�6   .+�5      �� $��	绿色：��6   $��Center=�6'   �� 5      -   5        $ GreenSlider��5      $ 0Q�6.   $	 255Q�6:   $	��200��6H   6T   5      �� 5      $ 
Horizontal=�6   .+�5      �� $��	蓝色：��6   $��Center=�6'   �� 5      -   5        $ 
BlueSlider��5      $ 0Q�6-   $	 255Q�69   $	��200��6G   6S   5      �� 5      $ 
Horizontal=�6   $
��0,10q�62   .+�5      �� -   5        $��30��5      $��30��6$   $��2q�60    � Cursor$
 Hand��6;   ���	#FFFF0000
	 � Tag$	 255,0,0��5   0   
���   �$��1q�6R   5      �� 5      -   5        $��30��5      $��30��6$   $��2q�60   $
 Hand��6;   ���	#FF00FF00$	 0,255,0��5   0   
���   �$��1q�6R   5      �� 5      -   5        $��30��5      $��30��6$   $��2q�60   $
 Hand��6;   ���	#FF0000FF$	 0,0,255��5   0   
���   �$��1q�6R   5      �� 5       -   5        $��30��5       $��30��6$   $��2q�60   $
 Hand��6;   ���	#FFFFFF00$	 	255,255,0��5!   0   
���   �$��1q�6T   5"      �� 5#      -   5        $��30��5#      $��30��6$   $��2q�60   $
 Hand��6;   
�������$	 255, 255, 255��5$   ,   
���   �$��1q�6T   5%      5&      �� 5'      $��50��6   
�������$��1q�64   ��5(      �� *PreviewColor+�� 5)      $ 
Horizontal=�6   $��Right=�62   $
��0,10q�6N   .+�5*      �� -	   5        $��确定��5*      $��80��6&   61   �� 5+      -
   5        $��取消��5+      $��80��6&   $��5,0,0,0q�61   6B   5,      5-      5.      5/      