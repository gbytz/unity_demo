#import <Foundation/Foundation.h>
#import <SceneKit/SceneKit.h>
#import <JidoMaps/JidoMaps-Swift.h>
#import <ARKit/ARKit.h>
@class JidoSession;
@class ARSession;

typedef NS_ENUM(NSInteger, Mode)
{
    ModeMapping = 0,
    ModeLocalization = 1,
};

@interface JidoSessionWrapper : NSObject

+ (instancetype)sharedInstanceWithARSession:(ARSession *)session mapMode:(Mode)mode mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey screenHeight: (float)screenHeight screenWidth: (float)screenWidth;

+ (instancetype)sharedInstance;

+ (void)setUnityCallbackGameObject:(NSString*)objectName;
+ (void)setAssetLoadedCallbackFunction:(NSString*)functionName;
+ (void)setStatusUpdatedCallbackFunction:(NSString*)functionName;
+ (void)setStorePlacementCallbackFunction:(NSString*)functionName;
+ (void)setProgressCallbackFunction:(NSString*)functionName;

- (instancetype)initWithARSession:(ARSession *)session mapMode:(Mode)mode mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey screenHeight: (float)screenHeight screenWidth: (float)screenWidth;

- (void)uploadAssets:(NSArray*)array;

- (void)updateWithFrame:(ARFrame*)frame;

- (void) planeDetected:(ARAnchor*) anchor;

- (void) planeRemoved:(ARAnchor*) anchor;

- (void) planeUpdated:(ARAnchor*) anchor;

@end
