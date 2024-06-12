﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/127.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "88c4930d895a9ec8c6286eace83a4f5d73d3d704a357a06ad69cdd0912c15d4f6f41434f1dde623139591276bff497ddc5df9fef1743f5dc7b71312d52df4457" },
                { "af", "0a2419aa4883c2b8474fe34bf664ba16e4b2d44b81e9c5cc25b1e786942972d663b5188acfcdc5ebcf1eea548e89cedfa34f65cd1a73e78d63e6c13bc82b895b" },
                { "an", "dd1a181bb197a2f033478fcb8292f3fe252f80f5c92d6c6c3951d1a43bf37eca77bc285ed8b9799ebcd466ebc1137fe6665e3e040d73fb031de501443e540b06" },
                { "ar", "e520dc41c3e99b46ea68c767ed6d274302b2ea213380142c284e6b6ac570933f360e5cad24d8313a87989733495b1185c0435c89af275d83b04a21b2d17dbc52" },
                { "ast", "3aefc835f61bdc0dd732b7ab067d79e9ebac1d2c5ea257090e0219c9230bf2d87ed80e877f553c999d89ba617572daa40d2deedff07934a301de07d14a1e1e96" },
                { "az", "e4d43ebc42abafab9416abf25884eda9d42e011d23a3821ba57b7fae8dffffccdb4dd4fc0e1febe66388bc5791fd4a236675b9a6c4e16de42e1628953b2a81dc" },
                { "be", "f33fa92c56ba9b76d272f06d4b0d61223992a08b063240a3df712275ab5ef699dfc96cde4657da36095e3018feeb466578a8d45ea27142fe18c6beffeb71877b" },
                { "bg", "eab115360718b4c4122f33f18713e29c1c20aa9c8482513b090444a74d4bf9a94972bf69a716acb7f64fb5976946d2267ca13403b663029f29c53ae4dc9bb88d" },
                { "bn", "8fadce9f7733a5a8893c9f64de893103e1518fddcec0a9d3db776e7053d3fc00895a078ba384fd8a449486afb85cfdc994e178afe2b49fc8520101f4f98f9789" },
                { "br", "283ab1a6e8e1b08b59456a5b63c1e0bf0fd3e008a0845201dc5a36478d7c008a35461b34af9befd7b0ff27f11b83dcac672a4e1fe7200e4a9b68512c76a2eaa5" },
                { "bs", "8fbac822840cdd3e6996cbc79a85d3318c1b4e67d678ed833d43660937e07d37ef7db20b80f8fa598dc2fdc8bcc0c932a16b35f20b0bd1a46de8f9ce612e33ab" },
                { "ca", "87368433201b253f41436762726dca5eb23f9428d823e4d649570d7507602b5051c1f7d8d13812b7fdefc3a7ee058e17210a88f64434a8544a6c12be1e6c2ad8" },
                { "cak", "ebc5dabc8ae325b14b6f63a14495cdb768229ed82fa5fba42496a77fda32c45bdff6e12880289d119d96df16725b19cd55794817049573125299d613d4b7af99" },
                { "cs", "4abac7c8e584f676559b07227411cf260ef1a939272b484da289cef14ff3a38818753bf2eb76713e43b92ac436d610fc05de85af1a844532225ca3ad3fa2c5b7" },
                { "cy", "a2835342ae7752956c6030724d0ed8af9eb9035ae1d3eb347d14d80d329dec0fb8afa4129e1c76970270deee634ea8651650ddd0b7b45ef84637563144a7eada" },
                { "da", "382fb42c0b79e955050e005c72456cd53a5d6a6ebaebfb4bea82e01b3cdde256efa90860ec04c2ddd04dc8ca9c46d62bfaa9b4929275c2f6e3d4cc026eb01f8a" },
                { "de", "2f26a43fe0ae9ce6f3702317e9d13af491a36767ac5372b6976e1baddf163e9d59e9b9109f646d74353aa65bbb1ca1ba38b7ddce71f5d0c5d91c09909df562ed" },
                { "dsb", "521676b8cf271c3c79b243a12c1617b99caf457003bc2571951b0dd450f9910926baf7bf58d5b36552815fc0805dd67844df0e92ccc8c496ad2f561200bd969b" },
                { "el", "3fe19d3cd9ae0cb62600b2e362de174fff747f189dbb157ac75b05b496c2f67c392c042506b0e7a5b9f2d0bdda1747d94b86a8dd17633d38f384d8342b37b1ea" },
                { "en-CA", "21770ba3d9cd5d1d136ca2be6e0d870252bf0f8a34cb0622fded560d6da3b03386a1ff25b6e1323189e6e3bd214f57295a15049954693a5987c810fa66f28061" },
                { "en-GB", "6a9da372e8cb56cffd1ab5834ccdb26c2cf91ac1faa2f6207dc9f79900f0d9f76fe352a1778425ea167d7092e7115780be6913194fe1654fff6ab3635f5529b5" },
                { "en-US", "f0c87687cc11eae826e1def09585801ff5f151501e8e620797f33b7e7c4b9b4afdba24a6b8d3c7e32c26afd5bb733668674498b149cae6a912f9a40f7e3cf235" },
                { "eo", "c9b46eb4b5b391bac109325ebc191b25582a8dbd643e4c803564cf1f63623b9642ce900f13880c2eaad1e12a480c6b127ad079ef93c15948afb17471c9238eb6" },
                { "es-AR", "eb255d34bb0a4ad97c95226001eb7954e44c4a6b4d81527e6b7a3962e9fa7c0970e85f2a87446c2ffdc285be36df561d7f4e0865bba1c4ccf5265b0d0c6e2b3e" },
                { "es-CL", "ba02c6518a0ec9d3f5840643e0f885189dd153a9177dd2bd692cc93ecfe301594209b54e95aa4718e0e152dbc428a225895d13e0219b08428b3b5ebe70f9654a" },
                { "es-ES", "a6a800e3db08262e47d1e0bb70160fbb11df6bc02220e6135873831cae4a3e0de972e993642502c1787e3daf836d20bebe4fa8e5ff9f18249b2041276a65d1e3" },
                { "es-MX", "e876073893ff52aa8c7bf5c9a50b2cd048a010b8bd141bd8df52be69a3208602eba2fe0f573d11a9acbe2504a8263879e9411d04e27c4287b4e113adc7d83597" },
                { "et", "8aa1fd93bcbb52e6fdf7e684da428614d77906d6cb24e4f8a1280c51e0dab53aaf0bdbec76d45f3c9e78c55e666de1608a51c76253d95ad81bfa8b4ef3b749c0" },
                { "eu", "9b2bbda3243688ebd048bdd6d38ed15b487b8ee953a21d55a5a2f7612ea6b06e15f6ee6a310e36d621ae0f2b8bd8213fdc3d01ed3532d2718506383dd5ebcde4" },
                { "fa", "d108d5f16656da7e43291d16ebbf861fd1a673cec8386356a61149761e30af867d475035c3733701124d51abcd798fc2f8b72c1f71b406ca8e298daf3e8a9445" },
                { "ff", "dd3294f2008669ef054514e3998996b70ad129e8306046effeb29f65337e76f3451694a6ebdf9cc736ccf06b168f02dfef4c0335ba45610905853806047ac936" },
                { "fi", "1ebc1aebde0035735a2dca8cb30894a5e907f90d00a4e09ea582da1253884bd29c6829da18c8f55cc246d5557db1dd1f8f3145618954190db88fb1e56dce632e" },
                { "fr", "f1cb6adc371da80e0ef46c802d565b8b0700e10933c2a4a9bb2ea9fbaff797033769a724d7cb7cb1f5295cf569988ba12da8bf0858fe0416f4b4eb5b1ad5caaa" },
                { "fur", "09d8ed03e40b245b38f8ff9d2eb34a4a52a090a96593919b31e72d567883dc80ebee10b4e3fba2a43d27fed33aedc758fb8215216476924d25fcb7750d21d297" },
                { "fy-NL", "8d5b298016b1a3d3807e92644c9d805c8a0714711c4a621ddf2668576f1fcf9e94d4ceb5fac35d5895b4e832ef8819aee053f9a5a000cf9e10250f1bd7d1c967" },
                { "ga-IE", "817f7e2fdaf02f5e7e610f35b4ea55b75f2ff85ac68405e086b6adab7da57de62fd5d787f847ab383cdf601938a6d60f8d4b9d6e73c0d128d6b589f8230c4153" },
                { "gd", "8e5efba24faec517c50a779f4219f491ed8949a0cfc7e66b9d06d84b8df59fc5778f76ce38955f73fddc910f6ff29b3012ff7cdf6e4d001c43ae0022180ea1d7" },
                { "gl", "6de7119e2246d4cd0c6545bfb5baec172734e6559e6bc155fa506591acf315194af3a5f2c55a53df119282f0d53d23159d130fda669cbe073c9f82d2d471a380" },
                { "gn", "7df02cb54d73708e41ab074f4198cf17e67fa5c52c2df5398e3fadc6c260589fbb511483b6d1b65afdf68c398f62346de8539c895691ec99e5012436c2279916" },
                { "gu-IN", "64aeeecbc596ef1a6a3dc40b1494e9b33659e1680a4cfba7aff66126b67d70c20b6c4ab51355e7c04bd9e517fbc752c8d14429fd6c64c6cf27d2e607460c909e" },
                { "he", "3606a459f8637ad6d8467c30c0aa048c3be55ede8a5991d12130075f30db0a48d272f7ac1cbb257d10376e44b5d11b9d7a15ad3c6edab2206656831068658a7a" },
                { "hi-IN", "63f400d370397bcb3dd1e6b48a03b3e11a430ce11383e4a46e0c9ec3e439b216d4df70bb4147b44a12c00984b22ebb698c56cd62a5291b5b6d5d2428a7c4cfe7" },
                { "hr", "a512b6cf541dc63bfaa0d63ee61958bebd88beee3e97bfb522af45e5e5eefd8be0e28b18051160ecf05f460ec90ea9ebb7b2ba744aa2635e6fa860c8137f5079" },
                { "hsb", "84d5fed5d26fc67fa502ece84ca795f26ee38bb5abe36949c66693d45d4f0d99f0e01bc029b3307b1f4d9a3ec8053ff29be641edd0defc2296b9faf6bea68a59" },
                { "hu", "a13a9e1cb972e9b271ef8ebe36800925543f40a4cb2d77e0b1406624f775842e6549dcc5450ca7e62ca187c696a6a9333c7a84c62a5c8bee30c5f42a84a77dac" },
                { "hy-AM", "b8f60492a89c822016634c9b994d4a9c36150b2243107fac178072eea062eb0aa7792c4219a73656362cc5621f20f599323730db5d5663b7d3b00e0b2eb84373" },
                { "ia", "e77d6db23f8a3c8eecf28129d648892eb97ab88f92647e7dbac3cac87ad938379c645640deef9cc3d8b16ed8a4ca20c35a2c04b90acddd5abf697db8da9b9e1d" },
                { "id", "23b0a4743ce9793a6e199c9b30df0d8bd68086c4f6e531cc73fe9910417c17975c738c1935b077d07acb4bbdd071428f13de11786c4938a0fb35a896a07f509b" },
                { "is", "194e50d432e0490762599c4c3eeeaddea671f91645b0a2bbe7df7c2386eda8c1801ec9d03351fa6c45e377f2dba55574875973439a2a3dde8a01cfa18e1795bd" },
                { "it", "9ada081323a7049df96bb4c6437922287bf33ca990bbc0e7f2fd0939944ca3ab5819802414018ae31f62b8c747a13815935422b1882115fd9d4f87c3e31dd174" },
                { "ja", "7a0429e85249effafcd8338e1b14f4ac81160a2ff63a2c4e2018576958c3d0dd9323a0a24918689163fcafdd5546944944f4a80e2935dafe5a34412d47a0c908" },
                { "ka", "d5f1f9a911d4a6d65189d28b94da495fe8d7376e1dbc1c103e2371be93f1f7d4dbb1585afadbf049f17e63f494d8cc7a125502c5deef40c503ba20261c888e76" },
                { "kab", "1b0e01bcd720d9b4ae0a6439703b8fb610f4e13c08f6bfdc87fce7fec784959ebfa39eff13b23212c6a48bdb50cbb5b5f4aff3371ca54f4d36ce156e382be8fb" },
                { "kk", "9cc94b98142a857b628e099eee84ca56accee843748eb3ed6b44a3f6fb74c0114c43e14353cd27acd185a6d884426446787e396907b16873baa1cea2549fcbcc" },
                { "km", "610fb0c34349fcdc586676b52976a81ddd2619fbf6989c5b43821e6e4478211721d83dae8dcefad36e67755cd735ca502b8771ad5497cdb7dab5cff126365e41" },
                { "kn", "e44c1cfc61c7ca5cab985508c67d0393fc9aa213ed2f28dc431be4aa4f62d9dfc4183bb45eb91aa7457b11f6262f777f79e7ce2db52bd315fa0888138206522c" },
                { "ko", "56fa40f85ad8c55150f8ea058c003dc29621f2353ffce21bb6afbe8a30f2030e50905455c2dd8430eec7385510ce5c2b5951afe191265b239f4d671bd3fc8f0c" },
                { "lij", "6dc67e9e49c8e4098ffa02a47d0b8c6bdd15280631ab5fa2efac619862d248a7b5961e34ce09240a2f8de04d93523e6ae9e39da3f9f9448fd79ed2205e400d0c" },
                { "lt", "e3880dbe94f5382e067e4f94c0e3b0bda2a4cc3c94c7332d2d4c5142b359eec1278193026d04bb277684123a7ccba361f2014fef4590c7b6c3b874c2b77a8e80" },
                { "lv", "1f12169c8b82affe67cca7fe0888d47b89a35c3d2159e8ac9753d12b1d81810887c55977f80171c35f6b1e5e05bf17613a4ae249fceb40a184952b71f5ad6206" },
                { "mk", "517bb1e45d028be0ef7d3fd61b159e263d108c9cb68bf974fba1dd23a903df58d6f54ece4c08f4dbd25efd37db45b5ae7a7fe818e5a20091ca2a988d31e0bb24" },
                { "mr", "82df62886549e94560f533dc0fc663fefc7a2fd7207feeed74717d6a1c866b69129355c751987c5bc001993edf38fe2f0b977834f5312f30c6e6c09652d4f1ca" },
                { "ms", "aad47c0efecc944a87f70403773eb2f25bdf20d41fe70db5ade5d3176a6b0a7cf25f036e6ca94dc3c0ffff400a411d5e1979d96b0be4f757c8d49beaf554a7e6" },
                { "my", "60cf1c9cd9e4dd69ae024f0db520e797ab0c442b1ebb826387a5562bf0a894c49cb29044a1c3a828d304930ef19f878efe5bf9ede04e7314e7d96c7694600888" },
                { "nb-NO", "0d2c11aa5706cef219896b8db804b8f651b3ade1b8b18cbfcb50f2ad98a02d1f0be711409c1b917283c310b6443380de6892a66a879b3f3812dd245a007ebc96" },
                { "ne-NP", "4de12e9d5dbdb10bd8ba9c1a165b52955c959761487cd5871f1d61313b8dd1d02a426cf11bb2e7d7052abea364ebff62db17cd4d0e92dac9382fd6151c5257bd" },
                { "nl", "73988319ac4a015929f495569b612a643fe5b94dbb21cb145a307b9ba2bb33b0c6dec151dc3b0f5f7e6f55b80a788d69b50e39bc38b6170dcce07f46e29bf472" },
                { "nn-NO", "1bb1316b3f2ae6a34e9d0c4788ec7b4fb39fa8018bfaca507f1b8394d7dc46465cdf4beb658db9336cf3cac87cd764ffe90fe8932fbcee229e9c4952ecbce6cd" },
                { "oc", "e3294cd172aa89b4a5f8b4676fe0148cba219c22f4887c8b13556fa90514d3c141bb1b102522af5cde59ce0f23f12ce59a44aae6fa78d12c415167a93ab13d6f" },
                { "pa-IN", "47bfcd7ce594e09d59f76880d4fd0ccebcc1c3b2328dc8a5ccfb289155c3d7d0f17ed9ba14550df0fcdf9f178dbb012c828f618dc05282556c98933ebe73d96a" },
                { "pl", "d5da91103a89d3870ff05293ce4a3710f7a0abbb6064b1ec7d7d8aec10cd273887a30973872737d235fd08b95f44bc2d1b28aac0db4d43ff1b112d397233f40e" },
                { "pt-BR", "c90c18592ea11bd920bb9bd335b1921a9821f9a905cb2505be6dc721f92f071daca75623cb914b3d59e2ca79e1c80c1e1397a5b927bc29d3e600ff781415fd65" },
                { "pt-PT", "ebd8921c9b5ce9ca6bd5f2df9a319d60b794679cfb89e8bd3ae7ce1477915ac68a906744eb98ea8f5baf81f5292f142c9175e2bf617684f841eff7940e6223eb" },
                { "rm", "6dfb12fc04b6969ddb2c2bbc3c35a7c2eee8f99271f847635ee7cc5626321e9df5a1b6ed479e43f3d83316a606b762a9dff7b0c96f6dfd6fc3649e2a9acb93e9" },
                { "ro", "a491abcf81d1bd9dcbb242c20c13fdc6806a6a36fb453ee537704d43a89fc8c92ab99abe45b337dee0b13d59fc23fb31d71ab3e583b4843b1500a6601c66a6fc" },
                { "ru", "8662858c1e25e896e1a19d18da6023b25119bca294e28ba926610e02556b1c987bfee7b805d8a268fe2376cfd89db1d47d18101152590806cb5219187b6d05ee" },
                { "sat", "e9db269ca878e9d4d9549e3a74e2983cecff6191b264eee6e8ed2564fd8befffcf01c0d297669383a684b69bd791f231822924848d5b3766153d74e86e8c7de0" },
                { "sc", "7b2ad4ecf06f503ccc21b8816316023811e4d33dda6ae9a1924ff45cd55bf4bd142d83a1c5db115e89b09f53588c175a34efba2e42f26baafb8701c961ff4609" },
                { "sco", "af33b80f5b55f470cecf10b5add7cf585633235f0e2aceccc09969860fd09ad282dc8e29ba39cc481492721e299561aa6943ff98989ea480045048fc92e0a913" },
                { "si", "00516b2f24f8b5bd17b45c9a2a59a55fd27ee77e328c89b020ece616dddb755b182cdcdd12e1a8e37414c5a2ac763b422bf27849757efab9a05b36a41063e89c" },
                { "sk", "8db729407ccbb7d2520324afe1928f241801d4f10049b48808c7934c37e129e6120473580e9e48a9697af8cd003ada4ab26961535167412b9649d000008226c5" },
                { "sl", "5acbe75aac91e920daff27bb419332ca0880b0dda8cfc550c30d36910f194e2b981e4a65c5887014f820500e6dc1d9798d5632e84b26d4c2c55c57685f261df5" },
                { "son", "bd810ed0e09037e1dab334e1fb8f5f42bd186304d6fd68821febde18b95a494ed3a20973f60db5358c122b8ff739bf6727008bd05c9b4b8f552c28e6dc8e7f78" },
                { "sq", "7c4fd751dbf58954cb26a0adba45cdc7334647e40a27280cc52fd83691b4821b332bf795ea0f368fcbf0db092781c480d3b7ec3da5aee34295568a8e22dc0f48" },
                { "sr", "2576ab055a8d8b4bd7bad4a87291d018aafff05303d294248e299ecf5cb2f64a6cfcdeeff25224f3fe0426fd9b5758e531e73dc6e63391abe37aa6d9738f8559" },
                { "sv-SE", "868e5ed519e288ffa27fd7f086b35a1ed5455d260cef2e53aef629b07a9f91bb3da9b069c19765db9b69f7f5e28e2695bd70fb8a7f66c5218b3d981d8a7054c2" },
                { "szl", "fa405c44782aa98fc215f1d4a74dda28c064fc807aff941dead8d2dc492ed6dfde306d1e8a83ba41cf17823666ed3344b2fc63de87ac14d6624d856421bfbdc5" },
                { "ta", "49cdd56ce83bcb905f578267608e6007b02376005f5fce91ce19c9d03967e53f57ac2ace50303ed387c7ffc9c2c7e3c65a7e807c5ed93ff32a8dd2b5d08ecc2c" },
                { "te", "92e4e523e0ac9de6e93fcfe31bc1a4b90835be9a7a9820c10673343f926fcd2b9ac0063464176af9a393460f25912e65a7a8e7112dfa50242d42cfbeb520a2f8" },
                { "tg", "42353308f1698506ed4c81295fe42a254eb33dffd98ca914b7d09013f87f8feb0144aec21c823a761399380cbd68f01a022405a336b0cecca143c3e6266c9950" },
                { "th", "35e0783f449675a7133016041ab20c77f65f146631b149f458d4fe23daa9f1b2ca086550ba0accd3fada49e5d53f5c10908a66b562446c52134e59aeef0f305a" },
                { "tl", "039a5ea147d9c280a168c26186811608af6c987ac8afa35a53d5c86cabc873e5823e5e82cdbe2c642f05a4e67a7eb50fdf6093cc1809f2603131f72ba5de57fa" },
                { "tr", "1d20d5653435a04e329e071048e3a5d1aba59cccb906a746bb8218dbccb198b321d32d1baa737a0557c20f6f03ec80cff7b2c589f09b4a6f4497402b89fc5eab" },
                { "trs", "bfb8f49199feb5a2a0cc9a1bc1951a5ac8c02aaf7aeb6378ef70930edd8c6efb4514639805e1ca5e9450c8c35576855113488f9ad1303235a36a9d230dc69292" },
                { "uk", "d703bfee72c0e8abe9879298159cbfaae8ccc9e3a818632c591a16ca0eb0b948760392df1a905d2ee423fc1420c56e0963f829802ed24f5339c66f57b20e33b9" },
                { "ur", "187858aa9065c866aef152cd609adeac219254dab7ca77525c22586dba6cff8e85b42c3b4a46b8decdbd970d8da32328a5fce7ad0d9a8b1e719397106505d5e0" },
                { "uz", "3816cfbdfb2977785c0850f5c1c8b1978ba7efa78189db2e1650070d2b6fb7a029a0595eb0f427c5ff429aa3331490d03f477b31af1253d4caa2424c2b9f28ce" },
                { "vi", "03512c8c12b3d66b6c9fc474d055129ddd5ec3bdf0d795c93983fc30ced9374d0f487f87cc544cf5232b1243ff1175276ca6d8a6fd7cf0281cfffaeb66ff8fed" },
                { "xh", "b8a1f1510ff0bf4cd4bb0bfe5c5e9dbd9e97661ec5d78bb5625fb3a49e43e84506c0889c00c3809ea763c0415496bc2e9cbb808ff09328ec550dfa265b030b8a" },
                { "zh-CN", "f88c745c1ee879af450dace8e3c5ad0c6d639da534a936bd8381b29eeb9642448333bd248f35bacb3550dbc1d85fea58e4c9d109daeb19e5623dfc3cf2477717" },
                { "zh-TW", "17aed158dc5d90e09b6c28c22f04aa29ea0028d1d9855ec4a5d49de86c438b7817077a6126a401a90f946a506c311c4fea6c03c15399bf87eeb02e1fb4418e11" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/127.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "b66b397e8a881c64628afe4da89089ca2aa79c84ca67a3e330b0935bbb790838e7ca86ac13725a0574ac4dee849685674e2c73d77abeea23c23b665b39b01e2f" },
                { "af", "ce91784b49243ec435c5a4a35437fc703f3ba2e9bc6e6c3353a287173f5e13845254dc8ec666bcd2189530db119f567100f607c5ce2a3b41fd9b9a93eb50e81a" },
                { "an", "2c294d6c4815c013e823bf69bf630364202a9326ba7e93101474c5952a754367b87ac77590734defc30104fe6c6a42f352bee409d8ee653374d032281c55d321" },
                { "ar", "6e4d58eb7f1b39b798442b83ea6e99c5c73e4bf74187c5c36df2c1ab0b5933ecda7b57d02d69a426c84d66db15415eeac8081d1a11d1c57d6c1ed4f138672977" },
                { "ast", "7eb97b4d6018ff059c30962964bce19a1e617c1faf7e5e2e06e866fc13650a36a59e7cd26e9470f1be045cccd81d6be7293873b6488736009fa2800269c2c700" },
                { "az", "de6038e92ced110b05f54e2c33afa68a9643e0bf6382f54da01f96d203338d4580a823d8e2396b61a0757a9c11eff2df03c2afdd2cf6f955f0e3ba85ecaab46d" },
                { "be", "9f605ad1a2cc3197171e29c5b47c188fa4e99733d1d6dd302da9847155266ff1b1225762380e76d7b91c8b75377a4dbbed4003be591bd1e1d9445da5aaa3e370" },
                { "bg", "f2467673cf888ca9801b037d0cb1f89184d3d6b0d1deefb580f5fc4baaf329a048a0b1aafbc4029d337cac365791ad540ee9e9655df8e652a98a0d8f3dcee5c8" },
                { "bn", "c7aa17c31928ea5c50c35375b7705568dd34fa50c10e93660d2776b2073d1400a3991a82c3c68a161a5a272d6b11949643bdddf5dd19d7de8bd69d78b3a589c2" },
                { "br", "67de428d5ca8d6dbdcdfa6f9e760f78d5e620d7c58191c1bc4c6a8c87a1f3715ca50ac8b257be388fb22487c423c5fd19421ace4320e7ab5d9e4c9807fd6e52f" },
                { "bs", "6de3dada68812248a23f31db23f296b2f30084983edb819c460d3faa52657e44113b97ce0ebec9e07684594d25bc5db40351a0d22378752416cfb35b2a8b168e" },
                { "ca", "852a685cffa73e12e0e482f7995ddd7bac63fac57b62d31bd96faa4fa411c291fe259d424ed1a209f6c7d7f13faa1c8912d733f3e3f7c043b8d1debfb83e5b83" },
                { "cak", "2b87c5ec76eb54a69419910957527fa4b197cb3039a3ff96b6933830ed6fedb2969fb30800cdd20c57a24a0026db3ffd07683e8e8c59e037d3069805e8bc0a41" },
                { "cs", "6875cd1661640b909fc5ff461948b0d948038f53e326ce1387998fcade563cafae48f75582c020dc7b8412a44a45fdd497db97f6a3b0332747a0baf7ed84fea4" },
                { "cy", "6b33e403538ea0fee61d4ae5565e76b6e3ded906724b3d4517d1107502c7f0e76eb6d15b72916758cbe881e2f3fed1dd84e12ca982a2c756a9d65d257c599e3b" },
                { "da", "1c91683c73bf07c11c6c00fa7550ac4abac06fdb5ffe281f0949588193204fbf4a552dc0ebb5af244522d3c4941b9abdb4500ce92631b5f137b2dfc35e2dadba" },
                { "de", "0c56d161c345a7bd27569a24a7798047173717618bceac2cd2e51f964fb2526b1edd9bdbab0888fa2ce460cecadd3f9615d9232a90ace5c5e7a68884ccde5529" },
                { "dsb", "09af5cdbb82729ca20a150b1cec9432388e5169998abe24024fb156b361fe1fb19a3c0dac05b840add7adf87b48326595c9cf2515b4a8d4512035770084a6bf5" },
                { "el", "ac5bbd06d150dd556179424697619e09b2317992659ba567c7991afbcaa13dfe77051a2c6ee8884535a1617ece0997587a39b5de7a4e0717aab5d668139aad6f" },
                { "en-CA", "3a3c5e88e855287ef4458cfb3c077234224d9db4e88f72d7f30a92a8389fc04d5fe8db6b4bc18f29a6a8c8c7c2cb41d4a42b8c633a43ad39f9788ac5c2d53ff3" },
                { "en-GB", "ad6cf2550f98ebc1015c17d8afaabbc80733dac5503aafefd3d9942fc3096f7e7250ce52007dda8ae3edf760643d98266a5ba589656874ccbfb750d5e1b9cd66" },
                { "en-US", "c17761c763ae7bdbe17941ca4d397987684b253dfd1c0751aba5858f16b1894ed93c89d5d293b30e8c55ccaf248d9beffcc3e0fb21e7dc5526d447da219d08e7" },
                { "eo", "d3e4cbc4ab6dfe919c4b88cd8bc8fc9cb27fa7e6e9d717804aa30707c6621a480cf9df0edb1bfa59190b905c27f5233b95d471d07bac89f57e5a8d9fbb4711f4" },
                { "es-AR", "2738f7fe1e27a6271f4fabfa83f602743be47669ea76acb9193de2009f01eab6de06c156624ab8d22570ea526b45c8678d6d544d3075874b8eab7146a0d70b17" },
                { "es-CL", "d0f4ce8a7aa2f462c7ae52ab61b0990356815930f8b845a8cd4a1202df0a1f21eb37fd12e01901c783091199a015dd030783198ffa81e180b9dcf8ae7ce685f2" },
                { "es-ES", "685b8096eeb5ca460b087b240fd500be733bf7b9df695298f44a2bcbf361567ffb5d3e75706993d103d409607204c926f49161331e80ba6e66e731777d2d25a2" },
                { "es-MX", "765118d3f27e49fbca5215100f5afb5620f9c7d0668575b12759f8f095563fa14d040777c609c3bc92e4141be1cf5771e8b8ce01c50cd5aff0a9e0ad9c45806e" },
                { "et", "0292293c5a7e12898370e8e9ff3e3ea7db7d3a71b1e2757fa6584be39b46efce8ac33e54f5723fbcb39310d95e7349e18be7be9b54a6f8cbd8f55df052e309bd" },
                { "eu", "27f0482ee1ce7b42daa95970bf751d05c48700da60aee3579ded8f2a2864c2f20d7a37f8eb6b718f16855a6e667e7f90b605aab4c5d94f3741747bb88f959a37" },
                { "fa", "fb3e167493274e50b844194adc5986177e39a9c8df42a98df88d6a8ad842114ce5f18d4ac16e28129fbdbc88891b665b008f90780839968d8963965c3c927369" },
                { "ff", "ceb38a5c607297d434d3c618b087f43ec39daee7057283d0b5904accbd3221dfdd0d08e0ec1707e013d12b3b5744300cb2f3faf70c049b893c51d17f26e1d35f" },
                { "fi", "9a55b1c9f99f30f5d70e33265a90095c40f84c72f1bae232c1ec724ac4906c9459b7757257141daddaa6abc9a2b0def8fbb82cc567ce9bc847c1147856fb5375" },
                { "fr", "cddbfb85014c9e785dd3842a975dc1918bcf185829f5c94796f8bcf4d07cc7c321f8840a18c9d4168293f72d283fc5afd0c5c2b7584c4ebab8e480459f7b6672" },
                { "fur", "b614d7c94bf13422e5acc0d7d45ba2cd15a6cd99f43bb19318a1868b4aaf2a0823c4f515d61d9e9c0f7bd6453311788fb6d2d439b45fa92c6dbdf171d9ba6607" },
                { "fy-NL", "32639023e5a28eede6ee5e2cde82b074ca52d73ad0a1229e834b615d5e008a966d6511f408b61f088b5f47ae56d69f0fd04fecce29318a4d0b22d086b71619a9" },
                { "ga-IE", "ce5b175ed5bacd6d44c28ffe732db40590f528ba605ce3b8e6e9b19902455c315fa2890536052c0d58767b358fe9fdfc0b42c5f5446794efacfc361248d7f127" },
                { "gd", "74a0edbf877c726ef043c4f797eb40351d57076fa335a7aaf8acb83231dc554197ee47c806121026e0c2f0b8e105b6cf13da61b328a78e51cef4ccb8c28a33d6" },
                { "gl", "ee780d075660da836717b510b3798b4659f498e604576cd963e7040f8593e401e78812e8e42fde259e0c81451b3a6a764c6efd74aaab262a3633b8367bc02cfb" },
                { "gn", "3cac5a83c6b09e099b3b4d0c5943bbcf723862ed2dde48306f50f76322ae31378fe9e5e7da8bccdc78f2086c3b275fba554fba8ebea244e4b99bb53f7aea732c" },
                { "gu-IN", "9fdcdc1ee4683f7871c6a99630c4c0a8ef1bcd4250cfece5d576066059b6fbcab79f84f12802a8ffaacd98fb1a4752d0d1ce34d736b20dc396a2ad04c9961214" },
                { "he", "39d8cf7b9791e8748fd996ae27c38b9ed51baddc116cce984377379973b5b265ba8a1b60f077a8e9bc7f92cb7ea2978648a67bcac3903c7a074564e07bdf3bd5" },
                { "hi-IN", "26e658a3060f918745d19bd5280012471c60116378745d862c8ad4ba95713b0e2f0cf68dccfb3b30925884c7e4dcb5b4cdeaf60bf97079ef35406bf10eb4f014" },
                { "hr", "c04540df09e6640aa7ee5f34e90c4cdf7a0f1fdae8ba5ca2fa931ae4920dbe27e221c72b3028eeba3029c2539a212e569ca1964ea775175fdcf8f1036a67bea9" },
                { "hsb", "6e11d083b2e57855aa5498c806bbcd95d2d59f42c38a4bf8bd9c47bcf8851b19cb2301b931fa30d03d1c17103885de4b34c5781760cf3ce9c685ce91108f889d" },
                { "hu", "77e08eb475e111b36148c9763d21a931cdf71ae85d255b1de6eb66109ccd1784785f865bed2675ef910d7cd387a4856628f931f2807c3a30464b8ba7c69630dc" },
                { "hy-AM", "b16a0252bda0232785b771a3f4bdad6d072c4fc0f85f23b525eb4ce59b6b998d433860a1114c4198b6bcbb9097f91797493d484ff556441449004af944497a16" },
                { "ia", "26860e66f0ffa1f4131dab9ebdf4e996ad7076b3e8771a111a38ec08e7f3d80b8c4cfc0143c2a826f9e603bcbdaae4c9d33df627cd0560849abc218afe0a3f9a" },
                { "id", "13252728d653dd38112f2f813fdbf73eb73a2989b834b63e8aec84a3203c3f9b7e94caef200473f19bad89569c59dc78b866879f1f29d83e55ce40c2151953b9" },
                { "is", "1ee8766f8f95413ee357d971ca83dbe70658a66de5f55b9ffc9c431ccc90188bd99d892313447800c3e283987a221beb72a41a26ba1434a27a2b0508791a9cb8" },
                { "it", "88b9897a65bcf815b36d13f0c2944c6f4f02f035cc1845ef2c85ad20ee846f483b84cdb006dd83796ded0d2987143b580eff0e3da80fe262454052d10cd6541f" },
                { "ja", "9006e217830f9e3d431f9932c2bb59dd10552b378c65f839350f2ffd59aee546ff1ea61ea8c69a4ba376868356fb17917ad34b4fd055251bf0f895cdb1f5e49a" },
                { "ka", "866fd0e4da6c13be616c2c2be038d38bc7e01aae7d3991a8980c6866fa86569907f89aa2be20ca3b63df3bd633face6da52f28e59ac153688268765fd923ee75" },
                { "kab", "2eaf55914e46cdf68955f6604943597a6883ea3ad87c74af763ad2790f7130ac59fe293cba2f094208c5015948b929f4f868b7476619b491113c0e79f6e1b161" },
                { "kk", "66869aa8e4f9e37db2eaef2bda5ee9a5408d906625cf0b2cd6c6eacf54af69eff24700e677900e534e2cdaec8db890cd965e1612825ae5e3137197c41fa93a86" },
                { "km", "6510f5253430ac586cb0010bf03c1b1135d7cd5051c3a066fa8fa16311fb9616f296cbe94936aeb189fe386f2a352dfad699f52226c9fcb5861668301f91e0b4" },
                { "kn", "d330a7a5c34878ff1e06ce60e7a6f356c16d092f89e9eb0dacdc43f508f79f0068be3822ff96d542b682091cc8e7320d2a9eac51726c919481a306a2eca5fa0b" },
                { "ko", "0ded94350939b37437551cea91493e5832f840eccc7a5d78631ae9141cb4e5d7b0c921e0f7e4df8e83aa61b4a1f806eb5ea548679023f4a18e2208ccc681294a" },
                { "lij", "522ef557b9200016c6d6dad4dec981f32182bccf15fdbea7fbdf0f4ff77777fecd74fc92c226ed37c8e1d8e82d170298a258124b0b42c09c184c35cd84f78b02" },
                { "lt", "db07f5039b85c0be9670f40a33e5c3ad80bdd81c8fec3c004251f6bf47827ca6254b914b34f957b45785f56e93940410e0c9f4e2b806034578b8215640f7b0ab" },
                { "lv", "91b22e2b9888dae368a3880f07a0add3f0204e52d1b2cdd0f1cc035f8557acd4c2b4c9ebf6fa987da7c1b6b0b1dc1f2ee1758b36db8a430dd5ddabd2b3d7cf1d" },
                { "mk", "5ea474a9bd134743a7d385334db13c680ab4968c26500719cc06f446b5dbfe3dcce86e716d0f099b6da8563572b122465157d9b8b153dee29193f4568886b9d0" },
                { "mr", "da44e8c9f1c80d7cd4ae2765e4f6528a6bfc71bc82df21a672d82b37ee52971f8bc593f00ed3ba968c84f90dafb23d697decac12ab84f8f1d4b194a315da2e61" },
                { "ms", "b9798c2414f112db82b2b219b2a45c7e5fa585ad2d5fe413ab42c557632e5c6bcf74f1d41bb260ef9d6568420faff9b031d90b5c69e5f1ec375dd87fa24a5326" },
                { "my", "068c96c15f26bb8c41f0b18146a383302b22b6cc578406264112bad7c5ed7076f63fe860cccad29dbd58af390add00c1315731ecd2a4cf174ee668f173f87b44" },
                { "nb-NO", "1bf511e6180da5fff7bccda06e477677c800fdee170922f95eb4c7e6888b631abd3b4190fcbd0a1ea45bfd14ca2ef14cdaa2e6587ef63cda0afaea58f70bc40d" },
                { "ne-NP", "dc6276116f368ffe73c89b3ddddafc9e4d8e53b1dde268be11149f8f8a2ec1ced1f81bccc70bdedf772a6d47b0b9ef205226f74de8357d3e1341d8a5230e9452" },
                { "nl", "72958789a4f349e883f841f68329c1b175640ad6acf0a76e8f54af348963fe6f361f5b8207545997a6a3f732a3f5df985cef878804f9d074292b4061b9dbf166" },
                { "nn-NO", "3340c0d54083a6c9cbd3ba72b4bbbc34cc84a3c86d4fb7bd3e3494d484e6c6ddf1f759c66108e0fa840c1103f9be82efbc7338714c9de3b5d31d439f362b0e26" },
                { "oc", "8d63db87e81b23ecc11d56dc42792844d7d1a91c47fb7341e8b99aa3fb7ae119cd320743cb795f4900105e506afd07edacb6901c59702c6aa487a8f5da64b873" },
                { "pa-IN", "83afc7dba3b738e81daa7adf49e532bcbfabce471daa2792f21ec36dd81b5a7cfa3d75852430598cca94d989a2cd226c4e8ce660cff9669023212ffb2c25d9d9" },
                { "pl", "e29569d3fa0ed814cdb0ea1f870094f51f5ff59f7379de0a5efb8b51127d13a2d6bdd1dc931d051af99b3914cede954176fef7e194edffbd5b669b2cea35d496" },
                { "pt-BR", "cb6f5a7f7fc4b292eeae902dfafb555b2bb890e29142a204b8d07c1e0537fda750d05acf3e5a69ef467165c9fb74e134caefacd21d363d74623f414e66fa103f" },
                { "pt-PT", "3f80097813772bb8f3d6f97c535fa69880cd9b851f1b660be17b672caf83e1dd959caa9f26fa11ec7a56091c544bb5ddb5f66ea45569496f61527608a2f2736f" },
                { "rm", "8b87464fa4b03718ce3e466fa1b395b5e946cff53269a71bf6c0ebbc3a12713f57f35fa3d05e3a9921f4bbb8b22a9bd733002c794e0a7cb7c18c9da53110268e" },
                { "ro", "0459c2153479a9e00c09e2356d4e01eab2c894d8afd463af12111b79000a5723e6f5a27dbf07c273683ab4c21a51638772f3c96e8ffb8294c3822765a99472aa" },
                { "ru", "f2469f7b684b281d91a9a9f77c87ba113af4948bf2573c34c7b41a7c05772e684b09c056b41b3876b3c7d08906d9179ed2281ee71a1013e853e63a6f2b4fb1c3" },
                { "sat", "26129c06af97030c645d9a0c51187e25ae6cdd195cdb2c189b8d3aa24f57199e0d90531016e67bf6dce8ab4b0b746829933e0cabdeea15559b33c1d4f5e0f567" },
                { "sc", "1b5460a3ff74344893afcfbe46a9f0179aebb15612bdc73f42cc6d49c8c38334d7f4813b75b20dc66ad9d5ad96a18608fa99a47d59781151fbf16253baac799b" },
                { "sco", "3951fede4cc168571a80fbc05e6bac13459c5ba6a3ed5596c08ee5866ab54383869b2b7de1794b6b8ab61c24d1257cfd9dc1836a29f4327afc437aca53ebef4d" },
                { "si", "1fbe1c0a3a897d91e33276aec83aaf0cfcc4a5033a8971b37dd606391046ec52ae5645d534a76c603a7f1031e2bb49ef4ae0d8aaf1ac3e388c7081f552a364d5" },
                { "sk", "e129b7c87a7be4d749eb0f63066762dc41279da0fbf11890117b179d764389e6df416ef2fcd9d3643fcc3b6fa0cd0678438646459eacc893e24447276a9e050e" },
                { "sl", "1fc1670ed4f2feeb97625b789fb0d85dd57b8b995296f357ac2227315ac2da83cf395b42f8945141476512c5554213ce0a68d5b994b9a570108c39ff6ecba023" },
                { "son", "3a93d3291badc184e2da69f6d7360a8d7a2724821b0427963ddc129817a94ef5bde3f48910e2d2531cc43ec103e76ebb0fe1bd2c164954f53582415141b1eb96" },
                { "sq", "9475c7421e66687a4713ca79a3404c56fcc23bb3b78c1c5b99beac63017b7d7ae16f75b55962c1b057b92fa3ee4993c10cbab19d5842ea3f837e0de3bd0f8c4e" },
                { "sr", "2d7a3c16c84ef94191535317ef3c218ed0f91aa42fed306012e67954eeabddf8f7b7e11e1b30e6139fc594630acac442e3308fc33a72909a4eaa50881b7658ee" },
                { "sv-SE", "a0958a82dd07756238291f8ae232d79f1ef558dfd1c745db0f28cb3a11df4e83b51df19c46cb5901258480c448a45e5a9aa84125c2d623621a830f4e11e9e76b" },
                { "szl", "807a3a397703169924624dcfe4ba5b7e6d37bd559e21e95dc9d81ba5af9f5be0a375a64771e89fb25a84932cfe939d223c856f0362e043fe5ff37832f1b89eb7" },
                { "ta", "f91f99d9efad5d7c510e8c852c319bb8475c5b44e99aa0145d4284e72207d8ff847482bcc61eb02f47fdfbf4b800c8559fbf594f1e9e801f733235587b71d9ad" },
                { "te", "b22e0be8782e0067521a133623c7a8c65546649da9aba15e57cd60f87515649bf9aa12768762a8185011147207c52168f14af57386397233c6df5b45f193836e" },
                { "tg", "669c20709938cbf87777773f3e2e06c3f5e72434b2a7b3ed58272cbc6f4d35e7544c3b7621f74400aca01ef4fb7dee72cf91b55e63214d54c6f4871dead308ab" },
                { "th", "08a3903b444b76ce0bdf95f1235ca6c54c6db4bac2611443d1d3a17711152ca2d91c606a6c38b3a5f3a2aebacf53eca9784bd460a4704ab9a561a873184d24c1" },
                { "tl", "418e3206d17f717d2e9f3db2aefb7fea07389bbf1da3ca185ef2c86f49d5fced7e81815507973f9c9b69c3ce33e98b1c7e01cdefb4e95eab2b75a50f670c757e" },
                { "tr", "a4c403ab868f87371163efb9c43f51fc75ea10c5f7868a0b654ba8cd812955f2f9e055b51b0a2e881c88972a545add1cfa3e8253ef2ac919878b123fcee143e4" },
                { "trs", "b674e351f7fbd466765a109202329d8ae1ca372a4de9cd1b3b88e22857bfc902e6b160511ef672ef064f3bf21bf8d486e335430013abc073aa38ad14696c2cbb" },
                { "uk", "bcdb1bc0aa9828b51546c5d21ec984021fad8413cfc23bea0c0e92b1b9e06b9cda7329543440489a7c9ba5a34f5cb2ea70de17325306510cd3436495e1b35555" },
                { "ur", "7821d3a72b44c5fd0db8f30e3b747c8c4fbf5e073078c6f7209b1720b35a08796c5af336c7d721e8bc3f30a66271c42187078e33e21adeba46bad49b7231e616" },
                { "uz", "4040377310dfe4fcd44fc63f70e7495d61fa62a0913a88745c0d0fc67354336c863e13edb08c73ece92df59094d31251430b5b6d50e51b5cc4468c63512f1f50" },
                { "vi", "199c33aaa7db5f9730867f2cb8b3f7573530b3044a30a82ade5be20e8a9e7ee20a824e23ef239c1befd4b84b3181188f4518d8efa7d8c42086b536cb7ece59f9" },
                { "xh", "aa396a1be43a5d3c79253bc266f3b77d3d302e653921724df279cd2daadfe8089e339cb35e9d1576ee5c5feb1bdb40a59f67cf37411bcbe724f5edc7c1ddbe8b" },
                { "zh-CN", "81e5dd96d65cdf155985caca3278d75a22045f1990cf0648b2fa1867b81c205b933e8befa9192a7c389423d6b652c527241dec9cadbfb2690b624f9767bad28a" },
                { "zh-TW", "0dae6d6196a3ed34f7b4c380103832d3202cf56791084b6a0da1a9530ff3f0faaf3dd336e765abcb03edffcf788be312f5f92f9bbab6023259583c4b3820171f" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "127.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
