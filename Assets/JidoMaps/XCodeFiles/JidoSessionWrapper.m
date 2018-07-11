#import <ARKit/ARKit.h>
#import "JidoSessionWrapper.h"
#import <JidoMaps/JidoMaps-Swift.h>
#import <SceneKit/SceneKit.h>

#define RADIANS_TO_DEGREES(radians) ((radians) * (180.0 / M_PI))

@interface JidoSessionWrapper()

@property (nonatomic, strong) JidoSession *jidoSession;
@property (nonatomic, strong) ARSession *session;
@property (nonatomic) Mode mode;
@property (nonatomic, strong) NSString *userId;
@property (nonatomic, strong) NSString *mapId;

@end

@implementation JidoSessionWrapper

static JidoSessionWrapper *instance = nil;
static NSString* unityCallbackGameObject = @"";
static NSString* assetLoadedCallback = @"";
static NSString* statusUpdatedCallback = @"";
static NSString* storePlacementCallback = @"";
static NSString* progressCallback = @"";
static NSString* objectDetectedCallback = @"";

+ (void)setUnityCallbackGameObject:(NSString *)objectName {
    unityCallbackGameObject = objectName;
}

+ (void)setStatusUpdatedCallbackFunction:(NSString *)functionName {
    statusUpdatedCallback = functionName;
}

+ (void)setAssetLoadedCallbackFunction:(NSString *)functionName {
    assetLoadedCallback = functionName;
}

+ (void)setStorePlacementCallbackFunction:(NSString*)functionName {
    storePlacementCallback = functionName;
}

+ (void)setProgressCallbackFunction:(NSString*)functionName {
    progressCallback = functionName;
}

+ (void)setObjectDetectedCallbackFunction:(NSString*)functionName {
    objectDetectedCallback = functionName;
}

+ (instancetype)sharedInstanceWithARSession:(ARSession *)session mapMode:(Mode)mode mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey screenHeight: (float)screenHeight screenWidth: (float)screenWidth;
{
    instance = [[self alloc] initWithARSession:session mapMode:mode mapId:mapId userId:userId developerKey:developerKey screenHeight:screenHeight screenWidth:screenWidth];
    
    return instance;
}

+ (instancetype)sharedInstance
{
    if (instance == nil)
    {
        NSLog(@"error: shared called before setup");
    }
    
    return instance;
}

- (instancetype)initWithARSession:(ARSession *)session mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey screenHeight: (float)screenHeight screenWidth: (float)screenWidth;
{
    return [self initWithARSession:session mapMode:ModeMapping mapId:mapId userId:userId developerKey:developerKey screenHeight:screenHeight screenWidth:screenWidth];
}

- (instancetype)initWithARSession:(ARSession *)session mapMode:(Mode)mode mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey screenHeight: (float)screenHeight screenWidth: (float)screenWidth;
{
    self = [super init];
    if (self)
    {
        self.session = session;
        self.mode = mode;
        self.mapId = mapId;
        self.userId = userId;
        
        self.jidoSession = [[JidoSession alloc] initWithArSession:self.session mapMode:mode userID:self.userId mapID:self.mapId developerKey:developerKey screenHeight:screenHeight screenWidth:screenWidth assetsFoundCallback:^(NSArray<MapAsset *> * assets) {
            
            NSMutableArray *assetData = [[NSMutableArray alloc] init];
            for (MapAsset *asset in assets)
            {
                NSArray *parts = [asset.matrix componentsSeparatedByString:@","];
                float theta = RADIANS_TO_DEGREES([parts[3] floatValue]);
                NSDictionary* dict = [NSMutableDictionary dictionary];
                [dict setValue:asset.assetID forKey:@"AssetId"];
                [dict setValue:@(asset.position.x) forKey:@"X"];
                [dict setValue:@(asset.position.y) forKey:@"Y"];
                [dict setValue:@(asset.position.z* -1) forKey:@"Z"];
                [dict setValue:@(asset.orientation - theta) forKey:@"Orientation"];
                
                [assetData addObject:dict];
            }
            
            NSDictionary* assetsDict = [NSMutableDictionary dictionary];
            [assetsDict setValue:assetData forKey:@"Assets"];
            
            NSError* error;
            
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:assetsDict
                                                               options:NSJSONWritingPrettyPrinted error:&error];
            
            NSString* json = [[NSString alloc] initWithData:jsonData encoding:NSASCIIStringEncoding];
            
            NSLog(@"%@", json);
            UnitySendMessage([unityCallbackGameObject cStringUsingEncoding:NSASCIIStringEncoding], [assetLoadedCallback cStringUsingEncoding:NSASCIIStringEncoding], [json cStringUsingEncoding:NSASCIIStringEncoding]);
        } progressCallback:^(NSInteger progressCount) {
            NSLog(@"progress: %li", progressCount);
            UnitySendMessage([unityCallbackGameObject cStringUsingEncoding:NSASCIIStringEncoding], [progressCallback cStringUsingEncoding:NSASCIIStringEncoding], [[NSString stringWithFormat:@"%ld", (long)progressCount] cStringUsingEncoding:NSASCIIStringEncoding]);
        } statusCallback:^(enum MapStatus mapStatus) {
            NSLog(@"mapStatus: %li", mapStatus);
            UnitySendMessage([unityCallbackGameObject cStringUsingEncoding:NSASCIIStringEncoding], [statusUpdatedCallback cStringUsingEncoding:NSASCIIStringEncoding], [[NSString stringWithFormat:@"%ld", (long)mapStatus] cStringUsingEncoding:NSASCIIStringEncoding]);
        } objectDetectedCallback:^(NSArray<DetectedObject *> * detectedObjects) {
            NSLog(@"object detected");
            NSMutableArray *detectedObjectData = [[NSMutableArray alloc] init];
            for (DetectedObject *detectedObject in detectedObjects)
            {
                NSDictionary* dict = [NSMutableDictionary dictionary];
                [dict setValue:detectedObject.name forKey:@"Name"];
                [dict setValue:@(detectedObject.center.x) forKey:@"X"];
                [dict setValue:@(detectedObject.center.y) forKey:@"Y"];
                [dict setValue:@(detectedObject.center.z) forKey:@"Z"];
                [dict setValue:@(detectedObject.width) forKey:@"Width"];
                [dict setValue:@(detectedObject.height) forKey:@"Height"];
                [dict setValue:@(detectedObject.depth) forKey:@"Depth"];
                [dict setValue:@(detectedObject.orientation) forKey:@"Orientation"];
                [dict setValue:@(detectedObject.seenCount) forKey:@"SeenCount"];
                [dict setValue:@(detectedObject.id) forKey:@"Id"];
                [dict setValue:@(detectedObject.confidence) forKey:@"Confidence"];
                [detectedObjectData addObject:dict];
            }
            
            if([detectedObjectData count] == 0) {
                return;
            }
            
            NSDictionary* objectsDict = [NSMutableDictionary dictionary];
            [objectsDict setValue:detectedObjectData forKey:@"Objects"];
            
            NSError* error;
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:objectsDict options:NSJSONWritingPrettyPrinted error:&error];
            NSString* json = [[NSString alloc] initWithData:jsonData encoding:NSASCIIStringEncoding];
            
            UnitySendMessage([unityCallbackGameObject cStringUsingEncoding:NSASCIIStringEncoding], [objectDetectedCallback cStringUsingEncoding:NSASCIIStringEncoding], [json cStringUsingEncoding:NSASCIIStringEncoding]);
        }];
    }
    
    return self;
}

- (void)uploadAssets:(NSArray*)array {
    
    BOOL result = [self.jidoSession storePlacementWithAssets:array callback:^(BOOL stored)
                   {
                       NSLog(@"model stored: %i", stored);
                       UnitySendMessage([unityCallbackGameObject cStringUsingEncoding:NSASCIIStringEncoding], [storePlacementCallback cStringUsingEncoding:NSASCIIStringEncoding], [[NSString stringWithFormat:@"%d", stored] cStringUsingEncoding:NSASCIIStringEncoding]);
                   }];
}

- (void)update:(ARFrame*) frame {
    [self.jidoSession updateWithFrame:frame];
}

- (void) planeDetected:(ARAnchor*) anchor {
    [self.jidoSession planeDetectedWithAnchor:anchor];
}

- (void) planeRemoved:(ARAnchor*) anchor {
    [self.jidoSession planeRemovedWithAnchor:anchor];
}

- (void) planeUpdated:(ARAnchor*) anchor {
    [self.jidoSession planeUpdatedWithAnchor:anchor];
}

- (void)dispose {
    [self.jidoSession dispose];
    self.jidoSession = nil;
}

@end
