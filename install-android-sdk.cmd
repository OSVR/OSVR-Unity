7z x osvr-android-ndk.tar.bz2
7z x osvr-android-ndk.tar -oAndroid/
del osvr-android-ndk.tar.bz2
del osvr-android-ndk.tar

set DEST=OSVR-Unity\Assets\Plugins\Android\libs\armeabi-v7a
move /Y Android\NDK\osvr\builds\armeabi-v7a\lib\libcrystax.so %DEST%
move /Y Android\NDK\osvr\builds\armeabi-v7a\lib\libgnustl_shared.so %DEST%
move /Y Android\NDK\osvr\builds\armeabi-v7a\lib\libjsoncpp.so %DEST%
move /Y Android\NDK\osvr\builds\armeabi-v7a\lib\libosvrClient.so %DEST%
move /Y Android\NDK\osvr\builds\armeabi-v7a\lib\libosvrClientKit.so %DEST%
move /Y Android\NDK\osvr\builds\armeabi-v7a\lib\libosvrCommon.so %DEST%
move /Y Android\NDK\osvr\builds\armeabi-v7a\lib\libosvrConnection.so %DEST%
move /Y Android\NDK\osvr\builds\armeabi-v7a\lib\libosvrUtil.so %DEST%
move /Y Android\NDK\osvr\builds\armeabi-v7a\lib\libusb1.0.so %DEST%
del /F /Q Android