   M S B A M L   `   `   `  ����  StickyNotes,    &StickyNotes.Controls.StickyNoteControl   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � WindowStyle$
  None=�5   	    � AllowsTransparency$
 True��5   	   
������  � Topmost$
 True��5   	    � SizeToContent$ WidthAndHeight=�5   	   .��5	      �� -   5         �� CornerRadius$ 8��5   K   ���#666$��1q�5       �� Padding$ 10q�5       J� 	CacheMode-  'System.Windows.Media.CacheModeConverter$ BitmapCache 66   ��5	      �� *BackgroundColor+   ColorToBrushConverter �� 	Converter# [   �� RelativeSource �� 	 �� AncestorType
	 Window
 � ContextMenu
 5   
   �� 5      .@�5      m� -   5        $��删除��5      6'   m� 5      -   5         m�Name$ PinMenuItem��5      $��固定到窗口��60   6?   m� 5      -   5        $ UnpinMenuItem��5      $��取消固定��62   $y�	Collapsed=�6T   5       � Triggers 5   
   :� 5       :� RoutedEvent$ 
MouseEnter��6   .[�5      �� .��5      �� .��5      W�  �� TargetProperty$ Opacity�5      	 W� To 0.95       g� Duration$ 0:0:0.2D�6   5      5      5      :� 5      $ 
MouseLeave��6   .[�5       �� .��5!      �� .��5"      W� $ Opacity�5#       15$      $ 0:0:0.2D�6   5%      5&      5'       J� Effect 5)   
   3  -System.Windows.Media.Effects.DropShadowEffect  5*         Color$ 	#40000000��6      Opacity$	 0.6Q�61      
BlurRadius$ 12Q�6?      ShadowDepth$ 4Q�6O   z�5,   
   T� 5-      -   5        MoveTransform��5-   !   ._�5/   
   �� .+�50      �� -   5         ��Name$ EditBox��52      $y�	Collapsed=�51       �� AcceptsReturn$
 True��53       �� TextWrapping$
 Wrap=�54      ��50      �� *
FontSize+��64   �� *NoteContent+ �� UpdateSourceTrigger$ PropertyChanged=�56      �� 57      -   5         ��Name$ DisplayTextBlock��57   6     NoteTextStyle#��[ 58      $
��Wrap=�5;      ��57      �� *
FontSize+��59      �� *NoteContent+5;      5<      5=      5>      