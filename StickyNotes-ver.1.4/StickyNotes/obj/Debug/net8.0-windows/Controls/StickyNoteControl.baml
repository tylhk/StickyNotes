   M S B A M L   `   `   `  ����  StickyNotes,    &StickyNotes.Controls.StickyNoteControl   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � WindowStyle$
  None=�5   	    � AllowsTransparency$
 True��5   	   
������  � Topmost$
 True��5   	    � SizeToContent$ WidthAndHeight=�5   	   .��5	      �� -   5         �� CornerRadius$ 5��5   K   ���#666$��1q�5       �� Padding$ 10q�5       J� 	CacheMode-  'System.Windows.Media.CacheModeConverter$ BitmapCache 66   ��5	      �� *BackgroundColor+   ColorToBrushConverter �� 	Converter# [   �� RelativeSource �� 	 �� AncestorType
	 Window
 � ContextMenu
 5   
   �� 5      .@�5      m� -   5        $��删除��5      6'   m� 5      -   5         m�Name$ PinMenuItem��5      $��固定到窗口��60   6?   m� 5      -   5        $ UnpinMenuItem��5      $��取消固定��62   $y�	Collapsed=�6T   5       � Triggers 5   
   :� 5       :� RoutedEvent$ 
MouseEnter��6   .[�5      �� .��5      �� .��5      W�  �� TargetProperty$ Opacity�5      	 W� To 0.95       g� Duration$ 0:0:0.2D�6   5      5      5      :� 5      $ 
MouseLeave��6   .[�5       �� .��5!      �� .��5"      W� $ Opacity�5#       15$      $ 0:0:0.2D�6   5%      5&      5'      z�5)   
   T� 5*      -   5        MoveTransform��5*   !   ._�5,   
   �� .+�5-      �� -   5         ��Name$ EditBox��5/      $y�	Collapsed=�5.       �� AcceptsReturn$
 True��50       �� TextWrapping$
 Wrap=�51      ��5-      �� *
FontSize+��64   �� *NoteContent+ �� UpdateSourceTrigger$ PropertyChanged=�53      �� 54      -   5         ��Name$ DisplayTextBlock��54   6     NoteTextStyle#��[ 55      $
��Wrap=�58      ��54      �� *
FontSize+��56      �� *NoteContent+58      59      5:      5;      