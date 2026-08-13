#import <ScreenCaptureKit/ScreenCaptureKit.h>
#import <CoreVideo/CoreVideo.h>

typedef void (*FrameCallback)(uint8_t* data, int width, int height);

// SCStreamOutput delegate that forwards frames to C#
@interface NeyrdStreamOutput : NSObject <SCStreamOutput>
@property (nonatomic, assign) FrameCallback callback;
@end

@implementation NeyrdStreamOutput
- (void)stream:(SCStream*)stream
    didOutputSampleBuffer:(CMSampleBufferRef)buffer
                   ofType:(SCStreamOutputType)type {
    if (type != SCStreamOutputTypeScreen) return;

    CVImageBufferRef imageBuffer = CMSampleBufferGetImageBuffer(buffer);
    if (!imageBuffer) return;

    CVPixelBufferLockBaseAddress(imageBuffer, kCVPixelBufferLock_ReadOnly);

    uint8_t* data   = (uint8_t*)CVPixelBufferGetBaseAddress(imageBuffer);
    int width       = (int)CVPixelBufferGetWidth(imageBuffer);
    int height      = (int)CVPixelBufferGetHeight(imageBuffer);

    if (self.callback) self.callback(data, width, height);

    CVPixelBufferUnlockBaseAddress(imageBuffer, kCVPixelBufferLock_ReadOnly);
}
@end

static SCStream*          g_stream  = nil;
static NeyrdStreamOutput* g_output  = nil;

void neyrd_start_capture(FrameCallback callback) {
    [SCShareableContent getShareableContentWithCompletionHandler:^(SCShareableContent* content, NSError* error) {
        if (error || content.displays.count == 0) return;

        SCDisplay* display = content.displays[0];

        SCContentFilter* filter =
            [[SCContentFilter alloc] initWithDisplay:display excludingWindows:@[]];

        SCStreamConfiguration* config = [[SCStreamConfiguration alloc] init];
        config.width               = display.width;
        config.height              = display.height;
        config.minimumFrameInterval = CMTimeMake(1, 30); // cap at 30 fps
        // BGRA matches the existing CoreGraphics pixel format
        config.pixelFormat         = kCVPixelFormatType_32BGRA;
        config.showsCursor         = YES;

        g_output          = [[NeyrdStreamOutput alloc] init];
        g_output.callback = callback;

        g_stream = [[SCStream alloc] initWithFilter:filter
                                      configuration:config
                                           delegate:nil];

        NSError* addErr = nil;
        [g_stream addStreamOutput:g_output
                             type:SCStreamOutputTypeScreen
               sampleHandlerQueue:dispatch_get_global_queue(QOS_CLASS_USER_INTERACTIVE, 0)
                            error:&addErr];

        [g_stream startCaptureWithCompletionHandler:^(NSError* startErr) {
            // stream is running; frames will arrive via the delegate
        }];
    }];
}

void neyrd_stop_capture(void) {
    [g_stream stopCaptureWithCompletionHandler:nil];
    g_stream = nil;
    g_output = nil;
}