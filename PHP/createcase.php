#!/usr/bin/php
<?php
function fopen_request_json($url, $username, $password, $data)
{
    $streamopt = array(
        'ssl' => array(
            'verify-peer' => false,
        ),
        'http' => array(
            'method' => 'POST',
            'ignore_errors' => true,
            'header' =>  array(
                'Authorization: Basic ' . base64_encode( $username . ':' . $password ),
                'Content-Type: application/json',
                'Accept: application/json',
            ),
            'content' => json_encode($data),
        ),
    );

    $context = stream_context_create($streamopt);
    $stream = fopen($url, 'rb', false, $context);
    $ret = stream_get_contents($stream);
    $meta = stream_get_meta_data($stream);
    if ($ret) {
        $ret = json_decode($ret);
    }
    return array($ret, $meta);
}

function buildBatch( $commands )
{
    return array(
        'Id' => '',
        'Commands' => $commands
    );
}

function buildCommand( $name, $parameters, $target = null )
{
    $command = array(
        'Name' => $name,
        'Parameters' => $parameters
    );
    if( $null != $target )
    {
        $command['Target'] = $target;
    }
    return $command;
}

function buildParameter( $name, $values )
{
    return array(
        'Name' => $name,
        'Values' => is_array($values) ? $values : array($values)
    );
}

function postCommandBatch( $options, $commandBatch )
{
    return fopen_request_json($options['url'].'/commands', $options['username'], $options['password'], $commandBatch);
}

function newIndividualAndCase()
{
    $individualId = uniqid();
    $caseId = uniqid();
    return buildBatch( 
        array(
            buildCommand('CreateIndividual', array(
                /* id used to refer to the individual when creating the case */
                buildParameter('ExternalSystemId', $individualId),
                buildParameter('FirstName', 'Foo'),
                buildParameter('LastName', 'Bar'),
                /* only first and last name is required, all available parameters are listed in API commands docs */
                buildParameter('HomeEmail', 'foo@bar.tm'),
                buildParameter('HomePhone', '+469928372832'),
                buildParameter('HomeAttention', 'Attention'),
                buildParameter('HomeStreet', 'Street'),
                buildParameter('HomeStreetNumber', '42'),
                buildParameter('HomeZip', 'S-12345'),
                buildParameter('HomeCity', 'City'),
                )
            ),
            buildCommand('CreateCase', array(
                buildParameter('Title', 'Test case from PHP'),
                buildParameter('Description', 'Test case with linked individual containing contact information'),
                buildParameter('Note', 'Test case from PHP'),
                buildParameter('Type', 'Support'),
                /* link to individual created in the same batch  */
                buildParameter('Individuals', $individualId),
                /* address info for the case to create, not necessarily the same as the address of the individual  */
                buildParameter('Attention', 'Attention'),
                buildParameter('Street', 'Street'),
                buildParameter('StreetNumber', '42'),
                buildParameter('Zip', 'S-12345'),
                buildParameter('City', 'City'),
                buildParameter('FinancialSystemId',$caseId)
                )
            ),
            buildCommand('CreateWorkOrder', array(
                buildParameter('Case',$caseId)
                )
            )
        )
    );
}

if( count($argv) != 4 )#|| !preg_match('^https?://.+/api$', $argv[1]) ) 
{
    echo "createcase.php https://remotex/api username password\n";
    exit(1);
}

$options = array(
    'url' => $argv[1],
    'username' => $argv[2],
    'password' => $argv[3]
);

$createIndividualAndCase = newIndividualAndCase();
list($resp, $meta) =  postCommandBatch( $options, $createIndividualAndCase );

echo "HTTP Response Information:\n";
var_dump($meta);
echo "\nCommandBatchResponse (contains errors or links to created entities):\n";
var_dump($resp);

echo "\nAll done.\n";
?>
