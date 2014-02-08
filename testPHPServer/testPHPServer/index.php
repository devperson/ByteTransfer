<?php
//call library  
require_once ('lib/nusoap.php');  
require_once ('lib/JSONDiskCache/JSONDiskCache.php');  

$URL       =  "http://testphpservice.e3w.ru/"; //"http://localhost:7278/";
$namespace = $URL . '?wsdl';
//using soap_server to create server object  
$server = new soap_server;  

$server->configureWSDL('hellotesting', $namespace);

$server->wsdl->addComplexType('PartialData',
    'complexType','struct', 'all', '',
    array(  'id' => array('name' => 'id', 'type' => 'xsd:int'),
            'Data' => array('name' => 'Data','type' => 'xsd:string'),
            'IsCompleted' => array('name' => 'IsCompleted','type' => 'xsd:boolean')   
    )
);

$server->wsdl->addComplexType('FileInfoData',
    'complexType','struct', 'all', '',
    array(  'FileName' => array('name' => 'FileName', 'type' => 'xsd:string'),
            'ChunkSize' => array('name' => 'ChunkSize','type' => 'xsd:int'),
            'ChunkCount' => array('name' => 'ChunkCount','type' => 'xsd:int'),   
            'LastChunkSize' => array('name' => 'LastChunkSize','type' => 'xsd:int')
    )
);


$server->register('PushVal'
                 ,array('inVal'=>'tns:PartialData')        
                 ,array()
                 ,$namespace,false
                 ,'rpc'
                 ,'encoded'
                 ,'Sample of embedded classes...' );

$server->register('GetVal'
                 ,array()             
                 ,array('result'=>'tns:PartialData')
                 ,$namespace,false
                 ,'rpc'
                 ,'encoded'
                 ,'Sample of embedded classes...' );

$server->register('PushFileInfo'
                 ,array('inVal'=>'tns:FileInfoData')        
                 ,array()
                 ,$namespace,false
                 ,'rpc'
                 ,'encoded'
                 ,'Sample of embedded classes...' );

$server->register('PullFileInfo'
                 ,array()             
                 ,array('result'=>'tns:FileInfoData')
                 ,$namespace,false
                 ,'rpc'
                 ,'encoded'
                 ,'Sample of embedded classes...' );

$server->register('SetRequiredPart'
                 ,array('inval'=>'xsd:int')
                 ,array()
                 ,$namespace,false
                 ,'rpc'
                 ,'encoded'
                 ,'Sample of embedded classes...' );

$server->register('GetRequiredPart'
                 ,array()
                 ,array('outval'=>'xsd:int')
                 ,$namespace,false
                 ,'rpc'
                 ,'encoded'
                 ,'Sample of embedded classes...' );

$server->register('ClearVal');


function PushVal($inVal)
{    
    $JSONDiskCache = new JSONDiskCache();
    
    $JSONDiskCache->set("pushedVal",$inVal,172800);                
}

function GetVal()
{    
    $JSONDiskCache = new JSONDiskCache();                       
    
    return $JSONDiskCache->get("pushedVal");
}

function PushFileInfo($fileInfo)
{
    $JSONDiskCache = new JSONDiskCache(); 
    $JSONDiskCache->set("fileinfo",$fileInfo,172800);
}

function PullFileInfo()
{
    $JSONDiskCache = new JSONDiskCache(); 
    return $JSONDiskCache->get("fileinfo");
}

function ClearVal()
{
    $JSONDiskCache = new JSONDiskCache(); 
    $JSONDiskCache->set("pushedVal",array());    
    $JSONDiskCache->set("requiredPart",0);
    $JSONDiskCache->set("fileinfo",array());
}

function SetRequiredPart($index)
{
    $JSONDiskCache = new JSONDiskCache();    
    $JSONDiskCache->set("requiredPart",$index,172800);
}

function GetRequiredPart()
{
    $JSONDiskCache = new JSONDiskCache();    
    return $JSONDiskCache->get("requiredPart");
}


$server->service($HTTP_RAW_POST_DATA);  
exit();  
?> 

