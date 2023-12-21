﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "122.0b2";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "3c46251234e3e42e5d5acab82b90e979eed7b23f6c11addfe26b6112bb95d6ac72673f15bf5a384c35a2653b753f35367fe5a80e442669aad460e35ae3a57d53" },
                { "af", "80afcbc34845f781cc5465fccb570e55dc306edeb61c965ec3315f08ba857a889aa791f0ec080972f2388be3fda3d694ea1a3da1250dd392593621b3de01213a" },
                { "an", "ebf23d505e8951deefba9131497a360e8b9e020feeb3998c17089495e82689ad1d40a5408fe432f6cea8e85f59fee214da6567f7860d1177507552eb14126b54" },
                { "ar", "6052688f51fd0a234a64d11b84f5ac2065665f780025d6b646dc3014fa7a939f559800c1af46f869e826b188ae8120087dc6a632856f131a1b5cb754e3f5fb00" },
                { "ast", "3ab041c7645a9284633b564c20e994077a0186dc5dd462c2b977c6d957e397d90a8de3cf8f1ae98043254f0acfa1fb3f284a1d83013f518102dde67305d51422" },
                { "az", "326b4d8d8c1fe948466d16a5e5100724aa38e57eb4d4b031adb215e5c5bf00224c8e688ad353fd858e509c4f2f009cf6c16b11cb69d3ad5b59c4d9e600689dd9" },
                { "be", "360d87f7284bf65014edd16d0910c2d0dfeca2fc2bf3f1d9f7de7f85bd1a113796aa28856da319464bedd051f5a4c725f6939acc66172af1093671b4e4eb6457" },
                { "bg", "5d010a6f8093a7f8b8b77f15d5554b2b13911f7b35fb1fd98dba99180942d13655210aa53e5b4e64109e74ffe97eebcae90ccdc313117b3068ba76c56fd676ff" },
                { "bn", "3ecc048468daed918e445d60a2b5f65f0a2fc65e44aee3590863e7a0ada68a8317e6fdde2d924a2acfe73f988d8ad1211a71314fb08113697cbbdaa26cb6951c" },
                { "br", "4ba905adde4e6af531b99e22e906957e4edb7cce255f8b53e21d78b8a4c07c20a8b25b2bee495e0a250a033a7d39fe8d604ca4ad72a05f91351c23f539362ab7" },
                { "bs", "ea011f121bdb198b15d18187eff6c805a26b3b75c2af7b7a5a4f7157a07316371aef24f16873f19506ac79b6a58c7afa799b89fbb8fb63c7e155b076ae0354e7" },
                { "ca", "35efee0ba945ff15c82e4170322e92fecbb0d40617f839c124296ea68bf64cb23fc326ebb008976299b2925894f9f0c7c108e035e283bf392fc855ab373db1d6" },
                { "cak", "a14c27e981b8f9114cad7f5ca11c6e36b00f7a8b8322c9d35898edc61afaa5f49793202d32ea43c3bd6f25b5d9f4cfbcffd68a21c21e9ffcc8ae201f0e5146f6" },
                { "cs", "f6db0798fc027a9f3a3ecb8e93aef989da1e0cb64813c6664e73c62dad51e55b7cd88899bb8d62c4a040dba53cc50eb1c39ea357dbd56bd65b70c7120a651ff7" },
                { "cy", "d9aa871e1d4cb70c8d53524edb4d9b3c4eeec5b3f567149646a39b373650321e4657b2ea0d3c6fcad5684c3b3226f7f57ed551574a60aceabb100a9cf4992436" },
                { "da", "2fed6cdea1059ad83b46646936bd4048f1c430fc09f8ac8d0c514b05630ef517990db10eaf788cdd326a277c0ed5eae79a587bd7205e53a9f25d16cd8d723c5d" },
                { "de", "bfdc9c4f0679d245b8e2ec4a84627e2c259385ac5a390d9cfb0176d6acc9d822ea591a192dfe81e2307d8a0adbeff6a04f4db78c2da9698a1ebee021947dd8aa" },
                { "dsb", "3d90c6fd30adc754ccac4b808759a39f2a49607851884e073c45852df8b99c3113ecfffb2a43e1658bfaa8d9bc82753d333350efdd8b385a016cc579f31aaccc" },
                { "el", "b4285e674e14897288d7cb19b6c7ddb7605d181d042c90b50da3cd1c29c852b0922fd76b959232a6b8bbc613e6d33597da670025a71702c2b699dea778926480" },
                { "en-CA", "a1a6343306c4806f49856c54e8c954662d577a08ac5ae6d52683134c935a50ecc2cb04889368570e7448335f4f0f9fba2afc5005de63b9fc9c2f880640c1e858" },
                { "en-GB", "cd2a5173b650f7987002d0807fc24940101926ccd922c0f70afaff9eda07931070fd0ab58305ad640d1963bd6a2e3df9c938927bd1c467e82f7d80d5c49921a3" },
                { "en-US", "df20597b17d6123606d34289643267c4870971772d09abf7d0bda5f8a83dc908ebaf9bceab760bab3a9060e47e5d3fa90f91fca715f2bedf4b0da8516675819f" },
                { "eo", "d000ff1c95a68f1e10000c7862e5c65cbeee0917fcb03c2a095fb39bf2bf4a4cfd1b26a454a495a28f96a552c0f5cc7d37b2ff6729a96627f7041f324de67931" },
                { "es-AR", "75d4c7fa7d6a454e6621600baf5e73aa7dbcdbe7f49eb687a271e9b3c7d0bc711a60853774bffcdf4bba0cab455de2cc32dc5dc5f306dcb75f375c7a95c27af1" },
                { "es-CL", "d3718831e7321145c225e56c2cf0e1ea5447e34455d08333fa1ce4941a6d35aa1aa0cd098cc34d89f631f682acd8efce4b9d5d55bc993f4fb4164290808ce0db" },
                { "es-ES", "eae116f13dae798c6b319f137e0a3f724ab61c2195b7c301f56a8c22fb143fe7effa661248b0dad844a0b1b575f202d296913462f3839ddb66ece5a6e0a74395" },
                { "es-MX", "bf512ed001b95c59ba3d130de4512d8d04f15a294bfec2f9b4d868e04a5b8db1c6f0946ba94da6635518bd24e0902a772f4474209d4f83e9106e1f9f7ff17d28" },
                { "et", "9a1338111e71ae73c404810cdf3af5a4b4b42ca1c2b711d0505e7b32fd1061fee6ce18ccf14d747e32fe8a44c809bd4547743248aef932595a8a60edf3822d82" },
                { "eu", "9c57468f225c7ad186aad2bef59762c6adcd6f22f5123905a8c6d354074df61b779964f88ec18b27664db57436fa1b47513bc1f1e9501d2aaa8eace9f4d891e9" },
                { "fa", "baa6e5ae1ecae428e8233b09f4c263598ddff9fa107aa7e34e97d3a4a54e6aaddc339905fabd7ba781ad93dd5d51141d647ca2e3cecc2086ad141693f99dffcc" },
                { "ff", "786706aa44922ebda6ca8ef0d6493ff43408ddff6f94f3e7cea705187c976a29a34d2777a9238f8ec1d57f7ccbfa1b9f58cd75e19ffcf23e0a0e454ae8408c59" },
                { "fi", "a3d0e097cf2c4e235cc9643efc069a67cd9b1ccdb176cd24f73d970fa225fa737fad0941b035b7e151897e006bc4f10960165a6d00ffdd25b89f97b3225e4d78" },
                { "fr", "067e80bc421acf8509f1ada94aa671db207abd8ba2f7f2dd2ce8a394b5c704e9068c25d8c50014f9fa5ff5823b332d078d72bf213b804e0d5e8973bc248cac00" },
                { "fur", "14c4ea3be1361e8e5f46b01479e120d927aa1998476f79f5ca80ec9c1724b48f191b609b34144bf66e5f92f478d1355ef4989883dbb78821b315eb1de2b4fff5" },
                { "fy-NL", "5c23e7c996ec2f9a16f262b0cf08f5a872bb457cb4d88a75023188fae66e1cca1d047395fb321f1030418171b1865dd9e17cc841a3b30c6065bbaeca27c7209b" },
                { "ga-IE", "7144afc5c63e239e60a10e292a9940ac8bcf4f2e8480266d35affc1e2d21ad91be4ea65d8a05d8fcaa47877b2af844ff5c5e93acf7bee3290fdf43019c67e8c7" },
                { "gd", "166ba62dfc75d2b3360596fe20017c77f0922862a21c338d3b1088a162e2ade890517f074d03d2de93c3e91cab05a928c9b48843a5fac80237650f37132f4009" },
                { "gl", "daa848964ad135b026cbd1e61072247d8186e8e6c97b15f60bd9746738fa327e70e89a2a766a3eec16ab8fe96f18a20372826861957740bdd7a8f52511911064" },
                { "gn", "eda24bdf57d6ccf34ac72e0510770d2dc5754323882ff5915753db8f9d3095d4d33c34577b5b135592454aa4f501b4c952d96ac3a8cabf0dac5e83cf935fc725" },
                { "gu-IN", "f496f2d355616dd9ee07f844009a06715e423667ebdf543a71a01e1630ff845b6ea5c826ed234e362f8c5b79ece920aeaab315f690128f90ec67190ed4b8e721" },
                { "he", "a207c1f23287737a76d0d7be96e312c951888ba576fd47cbd2a9d2eaa68be1ecdc5f1250a8b6ad4d9f7c5f58059d1fbd568fdc1ff5bed8bcc84e28f5658a2bf0" },
                { "hi-IN", "7402388252005774cee5257bfd7023cb3b70379f2ca479f16118d3986d1e2c77cc4556a34ca39a8c389a466e0b4c5ae9cfaa445b6709264977d23a9baa7a582a" },
                { "hr", "78f735de7f46811ff846588215773c1d453fff4b58b104cb0c520f32f895817b88e31e82ac3c8f5fa9fd83094d659169db7f4e866d50e6dd0009e06b6f1cb625" },
                { "hsb", "321ccd140361fc4fda9423ba0d9d320213e2b1d197836851c9b1a5e84a93ceacc80c1f5d8736ae2975d4bdea2edf306740486c0903c9bbd780573eebf649344d" },
                { "hu", "f28ef1bdb7050cc7934f37b2eb9a58ae16a4bb61063dffbef3bdb2e756d17a7a2d2ef6556b8fbd2364b320e8cbe9c260648c295f37c24a0ad01ead8980d77b9f" },
                { "hy-AM", "12c59628de6a6ea28bb01f098526a39f662bd5412fa27bd97e94f687025fbfcb62b3dacde4e1723041aacb4b461a42ab43eccbddf8cbce80f1aca2688a564053" },
                { "ia", "fd80057a7f75f2d4fe18411d21739648af46f5ce1dd487f64d6efb404bccde7cf0360155bbf28078c14d6f3b715d60bb7155216f0b48b9254f4bfdaaf71888dc" },
                { "id", "ddb91dc0fc5f68ab2a3b326d557bdfc84707e55e8896328629c930239a9ebf657a6b5913c7119b05623246ea756852cf4c56e3e39fa91778612c61e234d29012" },
                { "is", "47cb07209f70d868b350c465dbda5b47ce191135fc29db982b5b3833a85a94b629e23f7c9fef4b60540a9e100613694b3fe7c5348d1a403c828bebf36a5539f3" },
                { "it", "4f7869a622d3af6602720ab1bcb966c16fab6bf2d681fcf9d38cb972c245fd3dfbbd674b6ae457c9842245d4774f5c9650f6f26130a175876f4c2a2c261d8358" },
                { "ja", "725970e5129f04fe7cdd3a6914344a6eb5e47b8004ac234e1ac0d4765512cc804e4be44357520a6e7274844a9161c841bff53a1687415ca1af58609dbd706efb" },
                { "ka", "4f886905ef0c2358611155414461747cb826946bb052333082393e116d205dcff77c6dd4b39a13e6c58fd295981f100076a42107ea32bc71d984260689f3ec8e" },
                { "kab", "9f1e11300abe5823084ee58febf1198f3a36ec264aa34a895e8b7a20b8afadff77370ed3c52a15fa1116708d3b4f89717c26abfd627152c69ae6551c161ed79c" },
                { "kk", "d4aa7a6351a8e6d38e2d5b69f5d66fd9de641e70c5a46b2165c790612d9647574b1b061c9f95a4be069f509a6a88da53fa4826466762a7edd5b61ebf7f728781" },
                { "km", "500efb43cf309fa107ee129936064d15631e64f33d5a0a1facef7d3870dc01040cd5385891ec9cdb839a3f4e557091a52faec09d795d61b70c735c992248ecac" },
                { "kn", "134b24f87fe79656263fd32d929334ca5a6f76001b51cfc9009b2ab304e9ddcb4fcb72a9db1950b84f9e3b959307666573bf4f8498d5e6759ebbb0f55d09965a" },
                { "ko", "0c8042bc49bc8991ce5f433b747c66d2e1e8bcc29afc4b80f9ec3151be7e6e19a558ab3e2c63dddb4f0a8c8f8d6b1cfb0e558e5528f926a8ba0f927eff389d59" },
                { "lij", "f63be0b64014d29ccf5402ffe6b36f9a8a94133f40a496e692307c0a900653498ec145a2eaf5f67af588de5cabb76b34f07a45f6a308f1cdb2bcaf3400b7e511" },
                { "lt", "53699ac51cc11d89cd5ee30390741800a9ae8eb381348b9d49977b6052dc3fe7080800099020e8ce05be915818234af46e755bc6b8acff914bf4dddde9fce445" },
                { "lv", "322f3bb655b7abaed6764f5b62a88aa9e7620685f0e1315b3ed98fc0dd0c5f35d2bb7e50f52bf5bbe9398576474dcd874df0e2b5f7ee744efb28ae4178b54bb1" },
                { "mk", "0a8d0c76f725413e4b034790be3d9b02410820bbd4ff6028a58014b8663f90409565abf320e9cd0f30c3e2f8723543a5433df9754c8a6d79386ad14c2dfa67c7" },
                { "mr", "db34b0846ba0f653a8625d37f8eb55be3e8e745587a9bd757cbcc2ff74b876c4daf2e7c65ea1b26cb246fc7d775c123f8f174d5913530e48e7891c780945951a" },
                { "ms", "7a592c493be088c700e25fb8f44b6619a46913bde58989b45ea68e6ede74aa73ca627bf0598eb4118cb7e63c4dd7db4073d2ebb5c4f565c778cb3dc139e30ca6" },
                { "my", "c6032965454a668cd78725d521e720f19115c5ed1b097aa2c73fd579818e4ba6ed33deebc5a249706c74ba2db7508cc9bb33f045a3059630795ff20208dfba36" },
                { "nb-NO", "5bf6ca268ea4570d51c810466e63487a21c41b84d114ec3425f951ba6a313d0ffdd8fe0178aa4ba07857860617958fbf04fd7129585da86482c39a98b7eddf65" },
                { "ne-NP", "d81bfb66573a51b3f7b7b9d78dfa7e31370a73751a6c34cafae60404bed54e27de319f489d951042abacd5f1e19431af09fc7cf89c7ce9e4a540ec87d01918a6" },
                { "nl", "798f391f096785e8263b71ac9a84577a92dcc8b96da25834f6e59042a3f268392b0e37132e0bbcebed79eca5a4b82052dabdcd172c13e406e57b4adf45f43780" },
                { "nn-NO", "36558aea880a1305b52dc3dc58695229293baa3df044dd2ddb4ed3dc3f88928dd0254ccbdbb894d9a349b24546f656464718041fa2659093bfcdadbbb396af4c" },
                { "oc", "7d85cddf1a0b8b05a5dc4c0c587968d4ba23dd69b6fbc408b4b436afd20d6b9b879d7bf0ae84c2ee11a9851d39e5e27179cfa0ea4ea114d4730a156c9d1697ab" },
                { "pa-IN", "4673a8644d90c5fdd6fb8a45fd9dd860c72ed434dd8fe134f7c09c53eefd6c4a692c61eead3602cf49824939894f62aa94000b5a3dd7a9c5f91484535eda0271" },
                { "pl", "04625e185d16bb880dce834a79ee029d6428aaa1a4ec1c26459d19e3dd1d0f437711592944222a9ee4bb787fbeb74fb7ce0f4e98cf9bed2b8881b189f8d12269" },
                { "pt-BR", "6e27100420a9823b6b24bc884840b552755903903d696ad8b7ae7f3239bc053cf1f5ef7096bb7cfebaeeba12496e67c67ff72dfc3b42ca61f7f3f065469c7b23" },
                { "pt-PT", "9abd08b06ca75241df0118533b642228c8fd8298489848d18ed887e514b338d2e08088c31d8c214730bdf6fad3470d6c01238dc9c70dc42a86a9ce1edf84f128" },
                { "rm", "790c1495ff62d87e1b7560abf2d874741dedcaeb9476d252909b4f7380a880bde2667092ec388c35a1ff7d8db12cf5ab431063fbff2ed99b873d83fa643e01de" },
                { "ro", "95f10eada3d61989779b24f4042507406d27e9b5b776b78f58558b8fa6a392b7a543229d81d8750b035226cb7fd88bc07e999e61a89c4c054dacad6a8a2eaa5d" },
                { "ru", "40dcb11321c899131c18c2d7e73b8d87a30234afae414f79ec3a07d4335c37973799ae2996e3c6a288d5b868347be21c5a459007a227d172cecff3de08e06d94" },
                { "sat", "431567d06577c36da049cf160747dc33a75c57fdee0f5cd30f1ded240717e7caf16833bf5e267db70f823f5ace362ab20f370d2cf15a89d51ce3e388d130955c" },
                { "sc", "0968a3a8266ee9d69061d709463009a8e096e54993fa7423af3f10ab70892f8ffc7932b547e298ea45cf82d2d05b4a535905250459792b71992cc58ee2b7d260" },
                { "sco", "6d048b47cded7ed1b9889c3a01c22c6a3c03ff4cad456c5c25dfdf87838cd16313b2a22351268d6fc329d6ab3b615563346b32fc304e08d0f5cdfb3b31dced65" },
                { "si", "b2669d30b467b482d75477ae30d4f1bfa11797014db5899811e5fa8b362f8b1890da9578392c42900aee38a43ba3f36313b3e4b2a9b2834914cd8c506f2f7331" },
                { "sk", "a23c7059fc859572a105a572232cdda0801288a7aa398c2cb18128b8e95a6fb8239e3b38084cfe24b238f2e5c037547386f9f7af689f8cddf7e92d3f2e18cff3" },
                { "sl", "2ddf1cbd39d4d954586b922c16cd163fbfb12474ca750aea35f61581955e4c86df8eb822ccb344914b90b393e74ed223951668fcec696683da29ab55459576a1" },
                { "son", "719b39e96ba786d1b8b7aa73527d15708fba512d833c97df53aa55af4e0c34aad7ee31885807783cee9e2728a00d39c063ab353fd272fbbb4b021a634c1f338e" },
                { "sq", "e2c96ac1da9547324473b92d506dc4320af12fd5bc5831d20db7c5ce1f105b04e34e3d3752070988c4b0a56ee80449e5c45d631308c49d8603150b049f09db02" },
                { "sr", "4e5f76be22d32c7fcd52002c22220b80609bf048937d56e263515a0ac91c034ff113ecaeac1303e421457190f230633cb83619af525e8719f70a76ec09e65d68" },
                { "sv-SE", "1f35fdf528bc7ba5d22a905d11ccbf6e765ae38c7a1063e4dddfad9f2a3e14146df1182e22772ff74a5886c28af72714412161ac4298e9344369c844d2057746" },
                { "szl", "ed0f05e868716212c22e1b9fbaa1df8689193835f3e9c215d172344916dafac41fb9f401c19cdc2972457a1e5a26ee88770bd9563dc3ebf6228dffc1ac2bd345" },
                { "ta", "7675ac8f24b8fa6c70ec03e3eb14de87eddae6fa8107221a3cc11d9d0d2739f9e94a7386bd0ea13b756a91f2b406d9d314fed8d55248cb1e826bbd0702fd334d" },
                { "te", "1656e4bd4989d7509ff66df1551d2ff6f66faec5a76dd5b2bff9798e9ec549ddcc2beee61aa9a96ac19ac8632a08270041d18087c6ea50240195f4e1f5e9af81" },
                { "tg", "a178cda3d9aec8b9ea9c35bfe85d0399fa843ff14dd0b2de0481d1b18beb97b832589ff95c80dd2b889e1d79925ca8baec55e00e5faad9e7244d075474692283" },
                { "th", "4ba3e37a33fc0a701863917b3e6174e70a412064e5cf0fbf85b89593cf1c62f52de0aefac82d0932974ecfcde0b0e0ae9c6bfca15897efc5bccf85ba9e4f1da9" },
                { "tl", "16ba5dbd0237d42da1ae32c81b27db106801a4c15e57cacefaed679c91387c1bddaf0640af490d68cf1a80571d9bf4f08e5aead844c12617f1d74b986546cfe4" },
                { "tr", "dd4025fea99679316c044462944deaa9ffd050f88368a08d56473d9b36df7a0e5f1f15800de617f8437e1ba951bb8955f767666e22e4df799c7143e18f8e0b82" },
                { "trs", "010200f659b97d8437e7836c08bee488d132b7993f46a3b2be43b78c0475c5ea71f2882027aac83ddc089c5e8861aa0abb0c05ca41e1ea630d8b0a120deb392d" },
                { "uk", "acd64f1a87ff024dabb64b2dd33a9a39e139a678d9a5c9c57780ac311bac6621099463812abea0dc599f3802ef028498b93865bc11bfe5dee6f314447d4d811a" },
                { "ur", "4fe62392926c024423be47ba17de6e36b2651d31e019ab1e98589af1b3653e6c5d8635a237d110c9641aa54ec5fbb09da0577e39e664faf6741c630920d97fa1" },
                { "uz", "9fc8439611e5a2c60c5d2ce4ef341b519574f1f634a2ad383ca76e493068305a2d3bc9826dc6d22e10303f661b4562e5626ed56921897981c1f174d2335b8689" },
                { "vi", "9b6a4e9c7a6bad7be3a395e7a02552e6514c6827bdfa1d905ee55a0ec39ff1effc423cb4d3bdcf1d83d322d986158ba7854afea8dee093a864ca4a573e420f3f" },
                { "xh", "f645edffdbf29cfafbcff543ba82eb3a934914861295a01c28750700c5855f6631ab1e15d15ea0c2f5a812f6c031ca31073e8d2a4f528eda992248b43c9ef659" },
                { "zh-CN", "8cfcf7d01248938cf3b4772733877159c363c132e88299108e23d4850a8c9802c209cc9e696db46393bd0edafea1673d48716faaa7d7caf2fe3be63a10869e75" },
                { "zh-TW", "df72ff41bcc3e33ec6a42ea2751485a57e894acf2d34a2cedb3dd4c218c0b33b0243c8111959e640b0227e019c7692b9e67059d0865a4a1bdc0000dd7aa1378c" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "d5d91d798f5c648e282ff0f1a80b009fb8f39dee5a890d464f46114bb911270ce50d349e8921733acabf10f7c70eb9eaeeb543aaa5ec273f425cb26e5f9f490e" },
                { "af", "c556da20a06a9b716fdc31a4ee2a6dfe6d9206c52a3e5092250f7690eba56db4394b76653d09ef1a68765d551e74eefe50a335c612ba682f2d3c5d295b664f3b" },
                { "an", "a302770cdd7d8fd56262657b839065bdff76b959b4cd5b88b1dc32c9104136c7fe0fdec1eb26d01d44505943772c72286ec8eaf88dc9179d5127b5b5c27d18d8" },
                { "ar", "54e75afda16d27dff74293d1ff2a3ba9849d528a2da9165736707a466f3d61d13ee0f2db48158fc8f1a9d84559e1d767ba055023dab32e3dd947f7518eac0466" },
                { "ast", "a82617a4457d9bc41da0869258f390919f15e4970e3fba0c526314ff1365f4b7dc9b2d85108705710c2e47bad26073ad1431f2460a52da5eebe65ab7ea675b7b" },
                { "az", "3a44a1b5690ee1346b7f2ae0d919a30e9395e6cda60141f04ae55d114a95d4ce249899fb059f5a4538cb0d106a95f6d759db3d93b6c150179871532720613295" },
                { "be", "f532e4d152562b57c3f3a8a3d480d3a5e68444c303ce327da21d341d0ce9470e712a93eed9c6f2b6dd66baa03779f3766b34b4c7d6add512f46d9cdac378fa30" },
                { "bg", "161b35fd808f6fa1af56344a8968f33c5574fe28b41c55b0d310d4e743fbecef30aff3781976cb61c0baceb3bbaa4266f33cc1b76b90661d72ee1021475a5566" },
                { "bn", "9491823c194e3d24deac77465f6a2568ad31ccc3a0f889edc7503a21b6aa6f3cdf0e67899a727449960e913f9db0b454be2ef81affc1913550efece7389ae7d9" },
                { "br", "4e3ff71c8082ba6864140efd9193af7e48c8b5782c8fe069b647a42d179b41f2d36d98171ac33fdfd217a4619b95a630f6e94e1eda894779019f9beb0689d75e" },
                { "bs", "7006987523f9be2aa716f47c49a0fe26b0ab96c98ec22d8fda4b3babd27b59429571b7613511644da610f8b2b3d10ee05537973cd7fee2a548cc26d16f1f9456" },
                { "ca", "49ac8e953e25f8a292bacb4ae1070f30ed9e2a20bc825c6d1f84deef0e65b0746deb217a26d8044eda5d4ceb5f939f33eb69b3f661910609104b346ce4da1051" },
                { "cak", "a0dd294396cbb091ee3a1f154e6274fa5b3f8b2d1b87b592ceb3181fc6c125cb61296d7bfef9bb6b84a7fee97f74d096c7c115f03239f87ee38c94d4fa088df1" },
                { "cs", "e441714ded6fbe6502235cad59c1a2b89fee250f21c32021cf0cd214237012d92773b1734c8727fcf393722a637c8a20ccbaa951c887822073147d41f5b51ee8" },
                { "cy", "63b58c711ae58028d22e5012ed182af8c952a083a19b89ef926f388d0dbbfeb76675277b10975329bc41f16e18d0b31a0454708abf8f2ae94b86bf73f06a44af" },
                { "da", "72addfe59d73f3578de47c29355d7bc539d7dd91766d8eb9ea3e4ccd53d2780f4e4935d56c3f4b205be99443afd6055d1f44ee01072a1152d9435f501f24f57a" },
                { "de", "89f6cd3ea3efad87fb15430b76a0f9849c93fae410bc2a093ea5bcc8c682443718f7b9ccd71bd661c9dc67a3dbbd8a82a49a3b3012dce1697a6e3d463ca95d8a" },
                { "dsb", "4e664d8b9973af16cdcd5b7436e2f3ab89f6a54f8570e46d58e41457de8ff8b9208ac2c5442977e7359ec425f0436a7e59b7f88426164a98e29b8ff03081f236" },
                { "el", "5eae8bbccc51a7303b405c3122ffdc12608c5ff5095556aaec9f561a98993c5f9ef1c03e18b3f09ca98176c60cfa3438ec9c5a32fad5d2ceb8c78bf8bb2b5888" },
                { "en-CA", "0731c6a86993911efa9bb1154e12797f7d69175c5c51b1738c49d072a39669f94c3a0246155885f67f5769a4690118f859fa4c623f3119db3e98f3b737dc81ff" },
                { "en-GB", "595ffcbcc1bd163580fd7ec78d3d50fb847ccd718c8202471de995af8517ef0dd223943044950efba25b2828475d2e117390a20e8b041e68355829f485d397cd" },
                { "en-US", "5380369356bb21674b7350d1adf397aa6fe71e018e19d7115d58b4c01e6726b418dc508dee1bd448b6c4ce3522429998f031c2e402e48b2555afe5b7e544fa8f" },
                { "eo", "cd7f8eba2540a67876ce8011b90511fa506fa61ed38cc1ec40b4772514e3312df86027d7188cbf8ea8131d6bbe19ec9e2284f666fa331e10f0dc5c4ed55e9e0a" },
                { "es-AR", "1a70bfcaf36856c84f6b82208106e778178d6a1db0d38544601ade7749cfbf1af907da225975aab2017863b1e5a1ea83ea19cdc0e82bf24002876e788df07d4d" },
                { "es-CL", "efc0705e4a433572781e4edb80aca9bf5f79b89fa8bd8ddf8e85801ef02b958b002b7e07db31060b2fe77acf300a0fc7ed4fca6453f75d4d691004015e15488c" },
                { "es-ES", "55c724c2fffdf57a8948350101a3e6996b98534862ff6c681c2c950eef4cd3622818123d6f82f8afc9b1eed007718fc726f66ab845044b969956d3e652e429fc" },
                { "es-MX", "c81cba6df29168b056db99bcc9e84ee91599b1bf9bb7a38e1bf3279103b833fa392f8d03f6b48539e82844f8696b425be4bc67775d15a972c704a419bf2623b6" },
                { "et", "0249608f422089e34fd791d9f6e5b060cd3b1ddc0cd6bf84095958e90c34516d6ce562ba821cba29ff5e4acd71f3c0a3713d9fd84646bcce1426e9b6515b6d90" },
                { "eu", "3eda53d1ba68412d3ac2bb2c904fb3693ffc1a606d5db1d3a96d6822a2ca24a937fbf8d5bfc554a73fb062291a4ba729ad26651c75242da07ca1582d88b9daf5" },
                { "fa", "1679cb65c0b8ec853392fe2efeab5240af6c7ffe877e6681eac7062469069e007a348d1f1383eeba8f13b644547114aff2ba970c9219706b81e96b2d2d4c5c8f" },
                { "ff", "458c954fdb581781d426b71d49b70a47d3d4c2484d6f8aa419a42739cb3fd15a3efaac99b9a147d01b6329b4c5576904afab04776354f326bd5e115b24eb927c" },
                { "fi", "f5c381f3e3005f1f79a1f05a38e37a92134964110242151cde6d1a60ab3511eaaebf72ac9746bcb38a4722fa02f9f3e7a52787aa2b11ca5de105a66934b9f55e" },
                { "fr", "0455682244ae6d82b265bc80576d21a749bd8eaa78431e32e20f0e435d538bf792d0cc10d1d926bc5883b37336fa8f0ef5dedc3c00fee1853aa4bd91e38e6d2f" },
                { "fur", "988736b772b1db980a7f32bf19199b364bc5046401ec572432b6095685a563200fb396236c5fa3704d6e2409d5fed672370974b82352cb7ddf690b526be01e40" },
                { "fy-NL", "60ec56771c7c0edad61dda29998adf787e7479ca46a88dae9deb61bb832fe70a4b09b5bbae9e04b4c719936cb523ef3bb578ec9ca485d63389bd1bd6aa1c7092" },
                { "ga-IE", "965df027671caf8e4aef1f0f01bad6eff31dd314c0a470dfcf3cc697606d5ec5d42a87287e94fc4c2949bfc7e62f6c17e64f69653593d150c14420102b97a13c" },
                { "gd", "57e1d49b284091cc7c4c2acd452425d48bfa35d6eb8a08afdfa1b4cd563827e4ae8917b7ed38ea5a95916480767c3747e97d2f9ff80d6286e38a588034e39181" },
                { "gl", "e0b0680363cfc30c5523f993fc1f9384c66ba81539f0f3bf15ad11acbedd3ddb35cc12f6e0d556d10193ffc167c1cd04ece89d3cae88cbe7e9d634f74a710a94" },
                { "gn", "4811efdbc9d011405ae5b16030eef6f0ae3628d2ffec4877eb290d909a3041be89a517068d6f44deae00351b994afee72a1103941e41727f7b094c08c3882b38" },
                { "gu-IN", "ba36ea43fd9e4167d274485135ebcdf26eba03192435de4e32b76cd72fd4322db0fca968c9c2df36aefa4de809f92b1a1ce05cf63c7779452b4e13288010fd9d" },
                { "he", "3a2cad589db9dd7ed57be5dbf518634625fcb2e89b240e15f3296a4bf7306a2890f893a3fe89f383b860b4d57d4e3259a6851c17fa21f9d7c36e28fa5b04168a" },
                { "hi-IN", "a64ed58cd24c1deeb47ae280f19179db42121f9df34484b6c11d35f0a8b932c7d028e526136d16af7a27438e96314b7f50ab162321e0ca7925e8760916d7112d" },
                { "hr", "3359489105a681e9c396bb91de5f3622ffedc6d3f7d162e7ab9c1e90882d96b20375352bd06d57f1bfada40964fb34bd0082b6af983df5b4a79ad1ebee907296" },
                { "hsb", "f1f281d0aedf67d980d2c2e9ba9139f9bceb989986d6e6c52771941ec7b3e710f3d72173348ed58ce841b247e44a56e381c66af63e448a2989135e1e4ab13e52" },
                { "hu", "c344b89c084c0f85c3fee4d9c8e591e11bade4b2e6ee27db5da2a2d2e78def60ee879aee0d794588bd7dfa80ef3b67cfb2157fc75a0f54a926479192b247c2a8" },
                { "hy-AM", "ff182e154b8e2b2ed4437c824383dc1c019c46e99b3abac2a4a0b756811548bf30600b075dc658511ef935f2602b61192fb4ab78eeba350c7b928b54466731e2" },
                { "ia", "4c690ecfc16eca4416e0f4234bcc90ecd4b2396fee38e507d72e05a452cec999cbe1f1de776c786b286d55d559dd2e6462c71759cce7584c86e5ff7ea031640a" },
                { "id", "dc8c7cf0165d2ec503d6de45b270baccdb11283829ceabbfc2c46fc74d7bbe522b3a4bb94959cd026261415d5fa98fe1e824a90129cecdb0983ad6d42462c1a2" },
                { "is", "3aefa2b638d6ee53f305fec01b69a5ffe5c5c15ce723024313f399c95b6945e5393d4a148f8329b25b2b7b184d368c6a30afc5a2bb7e88119f318a28742c3505" },
                { "it", "16be51c1e3a6e7554b30fafd5af02cfef65124f10ac4164b70dd46a06f5437886e1fe407fc0bd7cae6ed2cdd90f1e2b2250c456d4643238430c83f5ab90bf389" },
                { "ja", "93727141798d7996d7d1486b596d6f8ccf2707f987b47d4941f7e69610ae5717baa3493f19da9b133c9efda8778001ad3ef20b9a2777cb9015284c4d4c4c1019" },
                { "ka", "1d906f81d52c2c642fdfd0f099c3db7310a33f231bd4c199f1964e52cbfa84a170761b6002a4de055ebb28a50d3bdb355e8c3f3c9c0ec0862e9c3c0563ad49ff" },
                { "kab", "2c257c3590f19976809daf097dc616fc11e9153c975ec962ca3723e19c6b79ad303cf30dd11fc5b0018eb1dac4e6801a6afff49f2452acee4b3f123fbdc3e55b" },
                { "kk", "57cd1cbbbfee30302be7ed9273153d22bb5f6722c2c2a1ab61628e9bb2a46186e83c903bc3fa0ab6660bb21b383e5c6155ad512702ad0c2d01748e6950a7f3da" },
                { "km", "6da7da4aabbc437c8a887f558fd692f590234e13ea090dcc0c3000d6940a7281f6100f91062817e7d2b9878b200b3f0b2f6d4233ac95f06a7b3e5458d93edc53" },
                { "kn", "51902294064de95556d17aed962f545109eaa852c50d6e1b5c6722cecbd0b9181ed830cd7936d924e3c762086ccac72168a6a36f625dbd2da8c60898defbdadc" },
                { "ko", "d52411c20d8c4b28e16285b3b5a47c0ab0236ebd9d61b22717ca654eef260e03bf01831d388eb0fa672f7ac0787725bed65b803722d0641fa21015ed4839723c" },
                { "lij", "cd95cc98ab031936ad52a68a4c9e48f8f52bfafa64e6252e12a619e41ef215583cb5ea3d502dc5196471e5a56e4f0ad28266de4660e8b43690b4175ed48dee1d" },
                { "lt", "0b09289465301a543237b6fe11755305457bb52fa35258136a3336d9f31381d55daefe8849a7027046e146273f21c40fa59ca573f2d0fa7a2c4d68af42819c67" },
                { "lv", "21516b20b49e699bde37b785c43ae7af5125691b681e3c2057a07857d5f44425deb580df6b7c3822fd1d2251b772d38c0185100e02f73f086094ac30ad5287b9" },
                { "mk", "9647ea08da25f8a67300c07ec0a26d5419a37a4136dc3fc11c6dd86b39cfe848953b7823fed6dc9f7304d31b3e4578f6e4a126f06721e728c9821f3bc438005e" },
                { "mr", "ab3a3544d3e61d1609393507c023561149e62f69c1306432c2e2eb1888529522bb89ab85dad3f08f64403355d0061a4674f79d9818c01a43a95b987d4b371690" },
                { "ms", "33ae555cc4709addbdc3c6540ae3e11ceb665977263e5dc2a162b1388e36b096789e0c5e3ee8228269021a75296399a5f827fcf0982f3694c906b6d99fb56ac9" },
                { "my", "8c1b88b540c98689237a7cdbe62436f79616dac3bfddbe122447862480e3d7df5391a94621b3d6c9a8444e66b05b46c93c03e975b4f10c3d14bde5f49fcb377c" },
                { "nb-NO", "7bbf7c580eaccb0c6ce3aa9732824400d137af7d036d567197a6889428a366d2a37866d0f75d3492282703a99181d00b7dfc6070af9ef1c4a8cc23027ad63c4c" },
                { "ne-NP", "7930dd47db0bde6a9f0f91c8f57d3c3c87a8589aff3dd5108bf55ee4f985998cd571d11360d647ee33da832cdd3f60ac1a08530019b4a8a180b568ca97377e4a" },
                { "nl", "7c0cc7cbbf0652a8bb507b40833a5a229668144565b4149bc741c9bd370b6df26fb501681e27cbfc674c8314260e62cf5c3618d79feb84c2fb66a72207192710" },
                { "nn-NO", "a1eb5be77f817b34a58efce982b9d850a73d346e23c7cfda60b56c502a7b859e2ffee857b9038a3c43adf210c5d473009f44c8722bed6fd200abd4e3a242ddeb" },
                { "oc", "138a8c0bd6e2ffe1eb39c0fe694a9485c7337407deae3fe2817d285c78461553924c84bcc0d3b05d8a440f9f20eb669cf75c3c7c5ea815b8f89a2da7c3afa487" },
                { "pa-IN", "3266dbadbbcb47e5175b2515fc0daa482cea0dd33831cd982193d7c9b0fe2370807542fdfe1e0894e63c465ca1987be6bd87d8c43c5d702ce803a110f14ba9b4" },
                { "pl", "e2f267dc71828a7e2cb563707f9d492a785e65c253937ccbb960320e462d2f1a5cfa922b44c57b0e1d04a8fbe16c09a997a442b7f8ca2eec42b2f3e6738038db" },
                { "pt-BR", "b988a00fc9fa50bfb8f926fde0bb8dd79b4616852bf088b101cbdd0f7f9e219bb885aa0784f6c8d299d52c993b98d3f9f8af400857ff37400bc85b1e5b84f034" },
                { "pt-PT", "ecc8a9e5a4e4efe355951bbd7e29d211974bc02cfde6d259b5f329bfceb1c6616ad390d1dce28abb4dbd7b510e77706f69244588741bf642734ac4d53c540406" },
                { "rm", "75f30985b42285e6ffa331474c6136c343278d39cde1c9a09a0bc5aba6925eb3e88ad178dbd00635dcc19ce275217a191b158e5c15672d0bdd01b75514fdf2a9" },
                { "ro", "431b60318b8549348e42dc0a8ed0c2fa4ba0bbe4715e955680e0173ca34c36d0cdb5ea4b728ca00813bf0da31589a02b2ab0da845f64357df95df5057cff48e2" },
                { "ru", "ee5e3944f285a384adcffb9704d01aa3a3db3994c1deaa76e1ab17f32a8acacdd7d7397ffad7ef7d74e1f05545444c3e8ea398de1c97fedca8f3084d6d18556b" },
                { "sat", "92815fa85dcf1e030be838637b94952663817161561f1816a3b40289b6800ed8762ad7191049d3ef8fca14b4c689b1db7940feb758b4b02f11ae3aca9881004f" },
                { "sc", "8e3ec4f91ad4ee1de5153a94f578f2b0288d14f93aea2a123604010ea80c1bffd48c7cf754aba9e6ca4627d743b280370198f77d58f1a8978c4e17f3a3e4af18" },
                { "sco", "6a5de0585d271ce0c295125b1166b0f66eaed4a8bab71eb1933d99d94491544975dd2af42765dc865bcd2a1ebceeaffca9ace07e6c77fff75c644820414246a9" },
                { "si", "0b6ce3d4e492d8dc5e18e3d188379fc9b33905a8fccddb5cd45af36bedc8bb5846066116e8c03d993e0023ecf57b17bf268f759e8a7404a5686f651dbe14247d" },
                { "sk", "792cc948a723e227e81044446f8b3b3add12cb0ec58e44782100e061e0926fc37df75657646b933b52a15bfcbce86dbf35592b99c88281e4cbdecf15bea00eae" },
                { "sl", "ab8335f712eb21f2c54862354540680c327ee660631f84c61a20a765e13a382a31587a10ce60531464a494414de1877cc4a8f68b965dcc5e8c596ec3d1d80581" },
                { "son", "29100227e209713636abffb597441edc1320e6179593a79c39ee1552a023fa2aabc298dd09c3c3895db5d17018548a54271c54952113bbbd3474237edd6dd54e" },
                { "sq", "e6f6dd26e6b853cf3aedc47dff9aa3c54ea54a3ad7679f6b56254f9be2d7e5d6db929e33f0669d23c601fefa514d21fa72d83ea192172f9e2c7e8e8294d963a1" },
                { "sr", "2d73d2e4b2fd17edb865f07d8e69df4ee96e20e7ae5d0613569cc728c55ce22fcf673b4d8ef635423b034bbbef99bb2f8e2ceb909bd34d1e0819fdb8a92331fe" },
                { "sv-SE", "7a5a3c7de9b3cf848747745543eee05fd29b96206a8cfb529136684364b1432280009a66198ea788a08ca76606b60fbd569624614a5b2375e8d76bafdba7cf3c" },
                { "szl", "094b8acc61b9030fbef5d96451eff51019177bfeb845e5e87a00471858c1dc4d1e80cf548fdb220eefd05fd342ca71062f786de1d24ab14d4800a36ee81dc042" },
                { "ta", "9958298d5b5cc688eef3e703b080a9d3b78028f6367a70835dec9a8c9db16c95d04ca21dae2b4d3088864146d42217a7391c63baaefbc50c070dbb03cb7a9ceb" },
                { "te", "b05b3410ce07a9e0cc79e8cf35eb884f11df1b0e742e80a1561ed8e3cf4fe577620037808e302e6a626176ca4559821d6e472fe4035826302377e6c57e6cef8b" },
                { "tg", "79a3765da784840e794255559efd6b762ba17f7c2e27050d970dad04b7b34738a4ec88f23e5a3913ab8b0a27626838043068796eb983b1772d08f04ae29dc4f9" },
                { "th", "a365cecc6104a61b6ae5c73facbc3f724df571e0376a4042f3b24c7f0a2ad98c816a5c08a091777cce2394cd9cea93c353c3ab18a8d5c7db9dc7cf6d05d6ef41" },
                { "tl", "676e809730212d38d781213154d9f647ee250c1b1af4cfab790c660a9fcf9ce492dea9c899c8db479434c626c68f0927a20ff2578d99045edce687be0159a98e" },
                { "tr", "15b71b505b2025085fdecead735488ccd42bd518aa492700e19490f294c7c5c0b48f9f11e64c02334f2829b6f1233c295d7f433dfced6a4a35d36eeafecf7e67" },
                { "trs", "1cae4c183aeb2f316bd17eb5206d9d5846d1fb32cf55d2354a33877cdf37fa4ea47a4a41ba2e812fc46ccffaa105e7463362c4955b1550aca6ebf776a6983bc7" },
                { "uk", "2b92cd7417db0af4b657d140cd2bf64c9afeb6e9d8369956e6dab751ff78cadd8beec74c51982832b4301fd98d05fef4db4a8d9e16a76032c8cf74df7156cf03" },
                { "ur", "bbf685dfb225eb592765c1bc9fde4fb07884a59b5521099a0c91468d21c2172dfe07b6be92870a64d3d83ed9f620d4fba4ab210a647f61614f4ccbab5b9d5be9" },
                { "uz", "5ab291abe50dfe9ecfce4e55738eb3f7d5bbbe1de4239c7592a6d58f21f6e904601bb0fea34f5b9655c0f5b50770ec1b06a39cb73001b0849d87b85351e2a96e" },
                { "vi", "af49084fa10d38ba853698787c5788d3e70aa4540301d69eedbf7f7adf0357ef17b92a2ac54c0bece0bc9456300059393bc703db548662c7718477ef8b0c22c0" },
                { "xh", "e2ec88b1f7d4a94f369347038f2790838975a9f8c17b4ddc1290ac4bfa3bac6ebd449714894bc2743e5461eed601792e02e8f1f92da0ce1c1f0654b9bc714450" },
                { "zh-CN", "994562ad5628813b8eafa48c185be8cd0c0d98bb4e44ef960a0947b5ff1df6839c75b2776ebaaf7208b84772ea5e4f7e10504fdaf1fcbf9a747a9c71599df9a5" },
                { "zh-TW", "5699ec63841f98c3dac86f3bf7f24ce9bee4dfc96cf070418fc1508b4ea012b5c28820793ae517944527218faac0fa103a55474cad84e4ef91d1ffef618af323" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
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
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
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
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
                // fallback to known information
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
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
