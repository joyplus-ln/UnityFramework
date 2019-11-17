Prefs Editor v2.2.1

Thank you for using Prefs Editor. 

If you have any thoughts, comments, suggestions or otherwise then please contact us through our website or 
drop me a mail directly on mark_a_hewitt@yahoo.co.uk

Please consider rating this asset on the asset store.

Regards,
Mark Hewitt

For more details please visit: http://www.flipwebapps.com/ 
Check out also our other assets including the FREE Game Framework!

- - - - - - - - - -

QUICK START

The Prefs Editor window is available through the Unity Editor's Window Menu by selecting the "Prefs Editor" option.

The Prefs Editor window displays a list of all your current PlayerPrefs along with a toolbar that gives you common options 
for manipulating the items. Each PlayerPrefs item displayed shows it's type, followed by options for editing the key
and value, and then buttons to let you save changes or delete the prefs item.

To create a new Player Prefs item select the New... option from the Prefs Editor window and then select the type, key 
and value as you would if creating through the API.

To modify an existing Player Prefs item, update the corresponding key or value and then click the Save, or Save All icons.

To delete an existing Player Prefs item, click the delete icon next to the corresponding entry, or the Delete All... button.

If the PlayerPrefs change, then you will need to click the 'Refresh' button to update the list.

If using encrypted player prefs then enter a unique pass phrase of your choosing (recommended 10+ characters).

Please visit our website at http://www.flipwebapps.com/unity-assets/prefs-editor/ for more information.

- - - - - - - - - -

CHANGE LOG

v2.2.1
	- Fixes for Unity 2018.3

v2.2
	- Prefs Editor Window: Now supports EditorPrefs
	- Prefs Editor Window: Added a filter to limit results

v2.1
	- SecurePlayerPrefs: Namespace simplified from FlipWebApps.PrefsEditor to PrefsEditor. Do a rename of any references in your code.
	- SecurePlayerPrefs: Added GetBool / SetBool functions for getting / setting boolean preferences.
	- Fix for changed prefs location on Windows in Unity 5.5

v2.0.1
	- Fix: Additional editor conversion check when trying to load values caused by 3rd party values that are base64 encoded.

v2.0
	Support for encrypted player prefs including optional auto encryption of old unencrypted values

v1.0.1
	Fix for OSX path and key format

v1.0
	First public release