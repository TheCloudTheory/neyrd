clang -fobjc-arc -framework ScreenCaptureKit -framework CoreVideo -framework CoreMedia \
      -dynamiclib -o libneyrd_sckit.dylib neyrd_sckit.m