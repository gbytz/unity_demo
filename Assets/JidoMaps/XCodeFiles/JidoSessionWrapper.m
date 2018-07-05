#import "JidoSessionWrapper.h"
#import <JidoMaps/JidoMaps-Swift.h>
#import <SceneKit/SceneKit.h>
#import <ARKit/ARKit.h>

@interface JidoSessionWrapper()

@property (nonatomic, strong) JidoSession *mapSession;
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
        
        self.mapSession = [[JidoSession alloc] initWithArSession:self.session mapMode:mode userID:self.userId mapID:self.mapId developerKey:developerKey screenHeight:screenHeight screenWidth:screenWidth assetsFoundCallback:^(NSArray<MapAsset *> * assets) {
            
            NSMutableArray *assetData = [[NSMutableArray alloc] init];
            for (MapAsset *asset in assets)
            {
                NSDictionary* dict = [NSMutableDictionary dictionary];
                [dict setValue:asset.assetID forKey:@"AssetId"];
                [dict setValue:@(asset.position.x) forKey:@"X"];
                [dict setValue:@(asset.position.y) forKey:@"Y"];
                [dict setValue:@(asset.position.z) forKey:@"Z"];
                [dict setValue:@(asset.orientation) forKey:@"Orientation"];
                
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
            
        }];
    }
    
    return self;
}

- (void)uploadAssets:(NSArray*)array {
    
    BOOL result = [self.mapSession storePlacementWithAssets:array callback:^(BOOL stored)
                   {
                       NSLog(@"model stored: %i", stored);
                       UnitySendMessage([unityCallbackGameObject cStringUsingEncoding:NSASCIIStringEncoding], [storePlacementCallback cStringUsingEncoding:NSASCIIStringEncoding], [[NSString stringWithFormat:@"%d", stored] cStringUsingEncoding:NSASCIIStringEncoding]);
                   }];
}

int signum(float n) { return (n < 0) ? -1 : (n > 0) ? +1 : 0; }

-(SCNVector4)matrixToQuarternion:(matrix_float4x4)m
{
    simd_float4 col0 = m.columns[0];
    simd_float4 col1 = m.columns[1];
    simd_float4 col2 = m.columns[2];
    float w = sqrt(MAX(0, 1 + col0[0] + col1[1] + col2[2])) / 2;
    float x = sqrt(MAX(0, 1 + col0[0] - col1[1] - col2[2])) / 2;
    float y = sqrt(MAX(0, 1 - col0[0] + col1[1] - col2[2])) / 2;
    float z = sqrt(MAX(0, 1 - col0[0] - col1[1] + col2[2])) / 2;
    
    x *= signum(col2[1] - col1[2]);
    y *= signum(col0[2] - col2[0]);
    z *= signum(col1[0] - col0[1]);
    
    x = x / sqrt(1-w*w);
    y = y / sqrt(1-w*w);
    z = z / sqrt(1-w*w);
    w = 2 * acos(w);
    
    return SCNVector4Make(x * -1, y * -1, z * -1, w);
}

-(SCNNode*)getSCNNode:(ARAnchor*)anchor
{
    ARPlaneAnchor* planeAnchor = (ARPlaneAnchor*)anchor;
    SCNNode *node = [[SCNNode alloc] init];
    SCNNode *planeNode = [[SCNNode alloc] init];
    SCNBox *planeGeometry = [[SCNBox alloc] init];
    [planeGeometry setWidth:planeAnchor.extent.x];
    [planeGeometry setHeight:0];
    [planeGeometry setLength:planeAnchor.extent.z];
    [planeNode setGeometry:planeGeometry];
    [planeNode setPosition:SCNVector3Make(planeAnchor.center.x, planeAnchor.center.y, planeAnchor.center.z)];
    [node addChildNode:planeNode];
    [node setRotation:[self matrixToQuarternion:planeAnchor.transform]];
    
    return node;
}

- (void) updateWithFrame:(ARFrame*)frame
{
    [self.mapSession updateWithFrame:frame];
}

- (void) planeDetected:(ARAnchor*) anchor {
    [self.mapSession planeDetectedWithNode:[self getSCNNode:anchor] anchor:anchor];
}

- (void) planeRemoved:(ARAnchor*) anchor {
    [self.mapSession planeRemovedWithNode:[self getSCNNode:anchor] anchor:anchor];
}

- (void) planeUpdated:(ARAnchor*) anchor {
    [self.mapSession planeUpdatedWithNode:[self getSCNNode:anchor] anchor:anchor];
}

@end
