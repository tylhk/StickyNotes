   M S B A M L   `   `   `  ����  StickyNotes,    &StickyNotes.Controls.StickyNoteControl   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � WindowStyle$
  None=�5   	    � AllowsTransparency$
 True��5   	   
������  � Topmost$
 True��5   	    � SizeToContent$ WidthAndHeight=�5   	   .��5	      �� -   5        ���#FFFF99 �� CornerRadius$ 5��5	   "   ���#666$��1q�5
       �� Padding$ 10q�5       J� 	CacheMode-  'System.Windows.Media.CacheModeConverter$ BitmapCache 66    � ContextMenu 5   
   �� 5      .@�5      m� -   5        $��删除��5      6'   5       J� Effect 5   
   3  -System.Windows.Media.Effects.DropShadowEffect  5      	   Color$	 	#40000000��6   
   Opacity$	
 0.5Q�61      
BlurRadius$ 10Q�6?   z�5   
   T� 5      -   5        MoveTransform��5   !   ._�5   
   �� .+�5      �� -   5         ��Name$ EditBox��5      $y�	Collapsed=�5       �� AcceptsReturn$
 True��5       �� TextWrapping$
 Wrap=�5      ��5      �� *NoteContent+ �� UpdateSourceTrigger$ PropertyChanged=�5      �� 5      -   5         ��Name$ DisplayTextBlock��5      $
��Wrap=�5"      ��5       �� *NoteContent+5"      5#      5$      5%      