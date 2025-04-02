﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "138.0b2";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "7aeb0c87d58fd2701c55377e34d05a282a19afa8a2a4a4692d746022574ec5a39298b0fde32eed7c7e9b2039dcc28d826ab9f8e6f9d047e41eaf972f8a88914e" },
                { "af", "7d11693669ab2161aa31b164acd945f97ea12502fc452a74389bee0f76e9aa190e8dfd39ec16e11912271dfaa9d241844213555c8c5092763d2150f88bb106c2" },
                { "an", "99cafd85efbcfbfcf3ae4aac3f1d0a28dd80c7a744e1dbed2628a1abc2911e5632db3e00b394caca3b0ab344cbcd6de6429e3e8accb0b4e8f298dbe3445420d6" },
                { "ar", "d502782c918180b1dc5ef73a54c21a6ef0f22f88ef1a74094c2656e0ac08b842155f171c521498e5e4a486621c103f8a8cfd3c0a6e7748fe8da722545e308f7e" },
                { "ast", "0412ebd0e52e5ba973c78fdae2bff22d46b65e91d82159e2c4ee1d1c1a461950c653fb7ac0b236e82a8e6e5e173f50ea32d2c29738fd6049a93a0d2214b2b858" },
                { "az", "c8c592d50856310b709961d2249d416a9ecde313a9d23c054c37861559f70f2cd32c3922987afd2965db0fa57bcc7bd6c8d8f8e528c38c182a3b3809678e287f" },
                { "be", "c3ed306b3ef3daf6284cf886aa3ba7cc5a9881f26858966e0172400c8734cca4dd2e45a12293322ca9f1b7893b2cf17bc099dae41c25a197313cc7046d96825e" },
                { "bg", "c9aa21f57fd18d3c7d200566dea25ce53609c1bb43b7eec15693e25aab2fe44815231aed8f4c5012cd4831db4791514d03ace646169b3e8bba79bf90be6cb1da" },
                { "bn", "fe376ee1806b58e81fe5309bbc8c7d0c4496b27463f0b7971b06e7262b00fe7c25bbb114443a349cb3ee8167bb99c113a56f4003579f04a1886d3efb8427461a" },
                { "br", "6add8039a50578654928b4301f027c05a7d9b9a94903ed716f773ef97552b77ecc94b895ba308a79a1174b90d9d16eace1a7f0d01ac4a7c19abee306480efdba" },
                { "bs", "2cc2fea52bdff5bce691c46d9431bdc3776b106bc3b69651f62264a845e56ab57c3bfa714f29457e63dc32ac5f1b3cde77c7a777b7de1f472d9bf9b4d876ba71" },
                { "ca", "7e279dd3389f786705f8ad7aa37fd92e31a30e8fcab2eb26f20cec59d24e4f8c943f34f39baf173fee00b7a8b76378abba7c0e2f3f556bcd3315c9922218f0ef" },
                { "cak", "da69a12390ee4b606b44a79df22965b69dc71e95f345925245fab734da3438b4cd326ab059eff41025c7fbe5a03d5afaaef8d7a50a0f17ff1d752f148039587d" },
                { "cs", "2c2f4330c6b4dff5e315731cb9c20f5deda1ee21211240a4d379a63e684b654b517c346324a08c98c1e210279a6e313baafaac1644e9814e3085cd3566c00681" },
                { "cy", "1ff6bd04fcc16ea6ec51e2d735f2fc2212e2706f251c68178d84c4f7a6e34f7fec8dcf2514289d43ece65aa19a8dd1112ae6b818327f8cbb9432a4c216181342" },
                { "da", "bec10f669402d4e041733e2778b41548a4d3a3b7dcb97433ca1877f23efa0650d4c7b4ef0416ec9caff280f322a2dde6fdd445afe4dd194c9cd390dd19a917c2" },
                { "de", "915ac8069eb22c73a60ad82e5929d887158531cbe03921bfdf438f308664397a2f2259e726240d58cf2c68a80fa2ac92d3fdc0592f4580a847503780657d8af9" },
                { "dsb", "ec5070391bfc058f82b2a6230e11256796b06a71c1c33e6d2f26eef4a5616939dddef94fa3157f62781b8e4759cc92bf9b8eb4a742a613fd084e46676111b4f3" },
                { "el", "9de889556d5c44705c75eb85e739a4a55f771372be53952c402f55aa98094bff20f289ea229549e2b53495d56eb91bf30c51717a8b9fd546f4d5829cb7c38081" },
                { "en-CA", "8b9824ad5cfbdd8ba74fbc9ac9d5064c22604d6adfbde5b3d76636869362a22cd3e5e1f4a3cb846648122f38b4fefc46ee80b6b1c565849212e7a4269313047e" },
                { "en-GB", "e44b5b61dc5693f20dd4c620f8f0a85e31bd342724d1440a25595a407c3bba6fd1cf8c038652a5ebd94502180c5cc50aa2261f39640c0d3f1063d55bc571dfb4" },
                { "en-US", "0778670fd025f4e18c7744200c26a8b917deac7d06e5f1501eb87f2a8ca0bd4d78ee9ab963d2d3875c47b9fb346f8feb0b4ecdcba08410bbf797bfb04b92147a" },
                { "eo", "b2670202dc6141ceedc88591c9233bd2116b7652034390a726b3d29b86ba10bb93c505f84918504cb71a08590f4d1316420122a37b376eb3d7e727d8a8338cbe" },
                { "es-AR", "05aa831447954ce41fa772806346e1605d1f627e091874672164af842ccb54373ee642d88389484e15f8ed232bacc989d67c7d43958bb435fc3801e840e112f6" },
                { "es-CL", "59fbcc53a2c876c8388fc9057453a932f631c2497d62291e016fa06b3dbd7bba6d5ba743ac733ec5cf35bd6aad6b13197a82d53f80a7f0916c7f915c13736516" },
                { "es-ES", "a8046c78f138076cef958ed9764f037390bbd88cc10cbc0488121fd5501b0ac5ca7a973ed182b6fac9f5b7887170b035329ca2d2f20abe8323649e618d3f6d3f" },
                { "es-MX", "dc00c2d5c547039f16e08f25978cf98f319199a0370a1fa033708104cbc0e759e531c6ba71b898d5e6efec8d71c55f02af6fff05d68ee93d8317b373c4aba1c3" },
                { "et", "052f32c751b34e6ded22360d8fd95e75ce4c5e29832d4da536da8092ec09cd0d9b173d03eaf7356ebbb458a912cbf2210a4d57d5d892e72a06ae864b27b1e921" },
                { "eu", "06f5eb0c4e310f2f5cb0e850a331a4a8a8f10e97739604963bad39be13efa587f699eb040e3baf8a28f0e126a6dbc1d7d2faf5c74edb2cfef1231660ba0e9ed9" },
                { "fa", "19889b4fd6c530ed871377ad145bf87bcb31a55272f9c19425bca073a99d719d11221c981a0e76fe26eac4420faedc5fd6f98690b72385c5e36066c05f6a3bb5" },
                { "ff", "8cb8d3793ca9c0cc8d2303837c49de35ebb8d772a4bbf897c67d77ba7d78b35e109bcf1494309d2b5dcbe12ce876a5b1519989cc8d18208e3485c4d2b7c1bcc5" },
                { "fi", "9884d56a1a1085fa46038567bbce4ea9d1ec225e75105feb8b5ae54659625b699649163f35a26f3b252b1ac2d17dcf7b6120e3a75a65de83c7cd5af2bdff57f8" },
                { "fr", "7853a8463fa5d0f8fb006a0704a085bc8fc889b54717c07d691936c8854e21e35fc7c2dc819810d7db63f9e71638277a4e556dc0974673519aff7d1a6ebfb9bc" },
                { "fur", "1e417a3c23cdd0bd821e4c29da2abd9f9c32745d29f22507e68d7ddd5d494155540789a26a56661e020c06e8a0cfc24fd11d549d691edda3806fc8de61d87972" },
                { "fy-NL", "a026a1eddde97d001e8b7ee89d00ac74700b73717269565c876eadad197f689523b145d7b7cae2ba140b97a6a4a7a808c8d5b0eedfd352477532d9b24513fc60" },
                { "ga-IE", "9a0daf3d9395b34bb71077385a936456c697787fce982ea279e46ea6dfde5b28415338172fcf13bd5a74e052936690858b7e7722864a72aceaef5d3510e9311c" },
                { "gd", "b8dda61d24270d98342873e91d6dccb4e1fd97de41b5b0adf11b759ceb6418d5c3e7dc0b84df2c77ea80013870dd498942d0cba7a692177ec5f14db471b779ca" },
                { "gl", "841eccb366d757a56a536bcc5dec13393b9698110e99114eedb9391e31550a818552e26740dc002e3d0539d2a78a0b85479b00c27989122d34d243339b011a92" },
                { "gn", "a9f38948582a1e0f7acbd6303763f5856d3f1308e1bc91b3f4e4f403d60fddff304f3b37577d888a6e0c96bc8222dfbf94bc89afb4aed7469e92c4fc3e26d87e" },
                { "gu-IN", "af1a85b9fc0dc50eefb2c1c649668df7f4ed15a5c2e88ecb96c1ee2f25497277580d5b81f5b4cc8ab341b4713aebe960fa644a872efe06ff869a94b29464dd0e" },
                { "he", "c906e9ad692753077752c83fb85e2921b656d0102c2a677698a2ff2aadfb58296f42eb79395b2e6b0a0d3b01b2f436a072c2592b0790a8d51ea3f4fcee858ec6" },
                { "hi-IN", "6accf345329c6fbabeeaae96bd5a505b228679ae3eefd5c444cff24130c9fea5769e1fc26ee3c23e4c4ed0e51c53e39182366dc418140630fb2c6b8adeecbe6e" },
                { "hr", "ca7a3eb311c2688e2dbddf83eca7ad51add1e8002e1d67383a14efc1ef3f225cd86d661349aa6ec53b8b97659e015c9fab9b756293beaacf8a7423d5d3934322" },
                { "hsb", "634864e95b9b902ae4a23c30928d9b7a8819fe4d33d95808ffacc4fa86418e486b42378df54bd9e198ae96d577e1186e7aad38bbef09054618b8a68ee0b1d09b" },
                { "hu", "0ad65e5aee249dd14a498c484a02490a8e3e7b38681127c560cb0573baf2d18826a4eddf7d0070d52c0ae2bfbc39ffbb1e8da8153c90101b5c241db4a8d51bde" },
                { "hy-AM", "ec45a455a22dedec0aaab62c0a0c4836eadbadd2a8007a0391ea0cbab42757b09be6e74cec7bcfe9de45f5849b68d3e0ea2980bd15693116be80b3ed36dc77a5" },
                { "ia", "fcce12ed1552fddddc9af8d1c3b95c4b7c39256bc313c28fca22db9158744a761dccc733757ee8b1ec103456c4dabc0b5d22ee7a04789ff030779a225c75a2c5" },
                { "id", "10f7b145001388bf3ffd4822c23d54cd1f26e348f44f8883382c122960e20cec75747ac3e5c68f34ad7d52957e5728e4fb8a245d276511cbf0d500c88ffa8aca" },
                { "is", "bf68127247e24f719e29310e6b5ebfd891997aaf488c79f151990c0e9828c4de77e0a36aa8f0bc80a1d98164d7d553ff5d7e5b3a68eb8fe9d4746694d41f88a0" },
                { "it", "d37dbdf4576ae0d44c929837ad1a3518e2c3d8d206bf870a80c67a78bad45e1a4737f9cabb30c46d31bf7e8c89f4d4c62b6eabc26b0310a8d4e02ca71d86c447" },
                { "ja", "1c2d197fd1cf6250cf843e3fb3ad65ae580c130111943bfba90d3b1c754024e84644834ad5a22c04f9a192bbc537b0c0dd264eb5f489250bbe30bb5a943c61dc" },
                { "ka", "d60b0785e29242f9a383d7b2624585e478f6414dbba8c7e265eaa00f34e7d5f3ba82861b14ce7c596026036e2012b692456a7a6de8b7d59f8741b9add680d33e" },
                { "kab", "d15d2cfd871a9b8b64e6a2c428cf0909024d8c703b683ac4c3fa7e385af85d00f181313673e89db40bea0690681d6aac1ad2105a94ce7ec42c63fbf18c96c28d" },
                { "kk", "76372d8f427dc7c609dd84840a330341f69597c1b756434ec6edcf147a5a3cc665ae405c4cd36a7565d7fa597a31abcf86184ad8c6df4f1f1399f38c503381af" },
                { "km", "cdf2a6aed252af53c0513bddf4bf7429e8921efe3b9f3966c5db3ed26d787c4c9f39b72548ae336ccf5063099af6790a4ae92500f08b3406483db3359d9df2cf" },
                { "kn", "51d2d5e26a48a051a02103102c19a5374c902b47b254a2524ce582ebcd1ad2d015496ca43746e9a5fbc7db211d84b7fe810e65b336fb2cbf6ff81d77933521ee" },
                { "ko", "4e6ad2e782db199ae8c92fba336190538838207698261437a69639f9291bba7d551e6e376ecf47739b193eb288cf1736b35d085b04a20761b052d079f6e7a4ee" },
                { "lij", "1b13b54fd942f2de6afb91a8ac002a901d77bd4326721230ce9b520bb1f976a5b52a495cb462477429eecdbd57361776bd0dc8d940e287c9e27ee9b814d748d4" },
                { "lt", "094368cbd421c63e5824f64f20daa1273755aedf17eff317046fd23599d9256bf597b7d5739754a454cbfc67d1fae21eac12cacc53307a8cff493dfd1065a55b" },
                { "lv", "25a791696b01dfc8c79a2ad80eb99c47347737b1db53d04ffbe1ba9f4482877c5cb6ff3e09f218d994991af8f2afe7b45d9ce223c3b758aad39f316b71b82b56" },
                { "mk", "60ae7938fd7fd3f128e08ff11c754e930d752a79c54d24764622d0a8405a4db69245de86a54b4b0b7a25905903f6916d33585ab63b8256d372f87ab80d2f7072" },
                { "mr", "407d7b80489b43a7be9e520e670eea3005434a49d077f4c179b21966d275a4b084ec202db2aa1b265102aa9538af33c45b7a5133245efa7262204570b312ec9c" },
                { "ms", "3301a588d0c5bf0f0cfb8bd83caff7989bc3cedd2847e52e21856ab843b4a121c24fee91dd00f27faecdc318663cfbf4f39b7c05bb337c3a479af554749a9a76" },
                { "my", "0e031815f370c7612c79ca6b577c0c1c7c29a16c63a48000410e659658dd62860c3d0b7af4042ad602447c9dff09ca49ef856caac653053d1c46e45f2aefa583" },
                { "nb-NO", "31f3f78df854bc4c54e6f6b9b8d47421dde82fa75c6c0761fd51cbdb4bcbd9cfee14a2f6d12c58931117938fb06e3a19773ced257cecdfecefa587b607a30c29" },
                { "ne-NP", "e86b92868f7932b379bc25d94140ef7b3829de05707526039e5fa1e08ef886bee5b4ad5fc2da97da1bc6a31c57254b1401950b3528cf4e604a0ae5ee12b30480" },
                { "nl", "312f7388646e4c03a94a50a6b122aa7e2f5e845021d3032b84282cec7f77bd866e37a98d5f8d49caeed76e628eb7c5a5a26dc4caaf86728540ed10f49791d5f2" },
                { "nn-NO", "c7c2dbf5c557ce43c47f443d5890cb18248b6507bfb68692b719095f5c99bdad7c86afe65d8826fa7038c155e12743f86bd21ac50b4ba87af10698d49a794116" },
                { "oc", "676ff7780f9108e5e0afea7cff73bacfcf836b1757f70c0efeb9657e9cd37983f1c493f3b6ec376a2711443f2b5ac593df75f3da176f41043a19a85d5c0054bd" },
                { "pa-IN", "4fd76ffa53a12e6e356b75075a59f48c73f6b23ed3daa572d6d798e85f266ce3a82f49eb4305b503a90594269bea37795cc139f74de348ab09e04415354825eb" },
                { "pl", "64be3377dd35b3ba2196092c1849668174c25757de2513412d0edf19ac5ff087868e79933e4019317e90fa3ee09a673c8e2bedb35e1c6466a4823d2e4af514bc" },
                { "pt-BR", "03aa3b2c8abed52fa6a0f719fc7ded90945eb111f49c2f65bd88361639d81bfbfffa0ef06529712e98c95c1cd01e3c134226abb7a5dfc45133f8b660addd4c75" },
                { "pt-PT", "7e36835139ba54cd6528c07eaf1a93a4510e0fd597ff96307c86546c8fe44b40a9f4adc98bffac29d9e23162474f1fa3edbe9de0657adc3acc8897b5251ad354" },
                { "rm", "09939704522f673f7eb1f9387b52b0cbbab12fbc7907409a86e2e589aba7edcca89233aa3f2ac67014716fb328161ea2080b684306b035e108d2e2f13592596b" },
                { "ro", "d2ec4fc4377cf9993e1d00d35920f2f07e5f5416d62a5482325a83b68df6f959b5c90ca360ab959d00c04fcb06ec76700bac9aea3f711238ae2bb586c60bd3b2" },
                { "ru", "c58d685099a5a1c2141b561effeca772d8fb83f624c5e800e05e32141657f047fa7b97c9b4df6025e4b93285afdaf42f48a3ff42dd8699f3809e3f1e4106203a" },
                { "sat", "64dc0e87938e3654a42c13b9dcab98090ef226f857a1dfcf81a84b6d12cf7066047c8a305aea09fa390c58a3bc910e770b97590b0fce1e2d48cdeb85e79430e7" },
                { "sc", "5f0e03f78c771d3396fb1157c3e9645e7bd1cf122a28a96f606e6d0c1ba9a6c2f83fae80b77f4ae0959f6c3b44419a457d51cb4a9bcb2dece62f2b291dda16a8" },
                { "sco", "f3a220706143d8c3cfc5704eadf0e069211dd50701fb1dd8bd3af429cf2a59137d435c50248388f9ecb2169bdf0570e56b11a805db15816d6e964145da8dc98b" },
                { "si", "33f8c132b2e4d7de2e77290a487a9f4b23c5993a34cadc2aeee34e234c4b319c1c243982f9b55452e38d96bf3254d5e9973656e0d63227def58f5949598d32be" },
                { "sk", "be3c7601836073c436cb796ea3f2e41bfb1b5d12d7807c320f05f629473c4c7b66c4f5fc568343e0438c297a15e8314aff15ec72f65fd7b2a4c74be6ee78e43c" },
                { "skr", "95b6073104bb44c87bab438d84c3fba883dd1fac99f9d0d0fed21507afe3fcda4fb9bdc1eb68318ecb9c054cff95e578153ab7ea733ca2bfff529566446f530f" },
                { "sl", "28cffafa4334d123c160ec91c527bef9a7ee2c16d9db0ee7fe7c133b4faa925acae7f5e5b884915e51767be87df14a53e0bc5404e94f1fd81c72e64216eff3d0" },
                { "son", "79a0597e99710c443718400174d2fc5316ec102d29d28162d9c5ff64bd1a57fefed94685eadc76fc8059c2bea89a6a3bc3530ab51740c5309e203a69fe05723f" },
                { "sq", "543bc5c31eb62c6526a52f58e491cb9846473b0a4cf0e7a5a3dfd35a539f82b24e1639ee407dba6491698e7e5c6fb4d0480b04dc0adc94bd311215e572a02fcd" },
                { "sr", "a911ff0850ad50a643a09f5d5f51a64bdf7165b9acbd730c616a3b0c6f1df09dfcd2de958b9a20930f8088a68b29c02fc7c39d1d8f787c1399f33cd634a53a80" },
                { "sv-SE", "ded94f47805b145b653efe3205ae50d2dda1db6d2363a6b2dfcf66cc330b515a387a71652727aa7a56fadce374139c21866f65dd1b8f2f75aefa908afb653dd0" },
                { "szl", "6f95037f723ff6325fb4551e022cf695d1f6d38f44072ae1a6fe9521463b263515caf5589041ce138650649f84127d1c4e6f861c838852c51751a706cac1baad" },
                { "ta", "85aba25cbece1f4ae8fbc41736016c46caf86f972fdf0faa3bcf4b34fb6f4f4b08f156d0f02bf0fac135932bf408d429e92ed91f3c7b890b7ae017c558a72278" },
                { "te", "840fbfc1c90651971a93471b86b04bc8c6337188ee11e617e93629a607a3374ac8da76d36f6bfbebee800249fed8b5c0ee40bb3528a65faaf2930e99eea0bebb" },
                { "tg", "cc143041fe5021fc607580058834e3eb47d82ee696762f9268f9096fec97dd6c05c92d89a5657100862cb5d19350df41cf075aebbcfb177121aeeb00fc853c7c" },
                { "th", "da8f159b20d415515781d4153b6840f8285cb69bbbfef16dc1d712c22141ee1da68a4ab03bc2e7542187da324c6bca0b07a7465fa8254bd3516d2443a1c66263" },
                { "tl", "c8641deedb01df8def4dbc6676c89eb0e06c899fc9b0a3a224470ddb82757096711504b94117aa5f90aa7e7f5f39c321e448524656d5c02493fac7055b1fb4c5" },
                { "tr", "a3f94dccbbf461e9f55cf64dbc343c903a7991ea84673bebaca2520b294904ce04a976ad8ac0de7729642998f16f8994d7ccc0866327afcebc24b3ad575599ff" },
                { "trs", "aa856e14a7905ef94796c7bbbdabe2c20244da753f51f598e60b4018400cea466bfda2b1d1d8d110f984db47ad9770d800169eea53879a57649e927b34c7335f" },
                { "uk", "b09302d4cbf147b09f567e48c3727b663584c3fa814f96b622d31fcdd6a0fdd203a10e96113e656b47dad564367ba9705c44a7ee372d6ae8279ce0377fe80186" },
                { "ur", "f664d2bea48717d64a07ab86717ce7cc6f583edb1e764df1eba880c01f0c704409ac7e1633f35c980f446b7fc0b7eba6083696603bef6fbdc8fba77ddbdf67d0" },
                { "uz", "0afe716197bd0bff284c4e6803d7ee3ea57def1fe59f9ec03214f04a5ecc4b8e1c69e8b53641cfb1253b7259dede45b5c5b042e928ab62982aad21bdee52d949" },
                { "vi", "0fc76b8ae7d5f871c81f9b7cfc7fd8949523e6b187877a36f2bce3689d2538862193f0f0fe6bcbf8040ca76d0cab12420a701aa5174a007254c20f79a12de9c5" },
                { "xh", "18b47ce2dac2455e5df307da2621e788951d7a58740c9e0687ddea3ad9084dbad81cbe3cb0159ba6bdecbccd91a4f7a90f4feac5306fc1af6e8f931b873e0790" },
                { "zh-CN", "25dc3795ee005d1bb41d5b628a661ac4c107c130754e6cb6880b53c4c2b78d937e763083250a007824071ce383ca98304bbea3f1456ad47ef67fdaf681f2ee5e" },
                { "zh-TW", "b79906ba1e3b0992113f19a10ae91deb6dfd075f594e4a5732b179f99c31c3150c9b783ff449086c6cad59eb21a9f8e5b4452a8c8509be9965ae90935246a99a" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "a76d8ec165d48375295ffbaba3f32da8df247bee1e17ed42e5f33f2e7ca14ac21a7950ff9f0bc4d65a84316bab54ae71590423f4cbb7e00e5564ed793909f403" },
                { "af", "8bfe34376fdf5b0bcfd87817563fc7e005ba9e38f0032a53d958b851654dc57a1bd811732cd1250e53fa25cb1c0bbdb275772d5d381682fc7c38220fd521afcb" },
                { "an", "3f6622a3e27c8c34436e07544ce09687f25fa608f8c7f3dc27dbbc980af98975ae294211cf812d121a6a60be4f1713c583e10aa6f2a8395084255cb958366f72" },
                { "ar", "e5bf388edc8acc671dce0e93b5d6d162d47a9697be8e0f592971b091d6f9e5dc9048f317855ca0a90878f7546044535070c7354a5874bfbef9142173ff0e2a7f" },
                { "ast", "82a9029c096a8af4da1d60a8f0c081b447eed312bf09844841760205ff5f645381e16729d18af902f203b3ed55c2b19952b121ddfeae77f2595f81afd13aec69" },
                { "az", "82e1b7ec6c722e1b8e698fcb9d2f4a0887aa1c00c42444da141be904cd15668c14bacb18c54b6ba78a5a53cf86782d284f5e4bea57aa0e937322b618d9085bdd" },
                { "be", "85b2d0412f9db4043bf5c950443ab7b59b2598c0405d702ff6bd9b52683a9a56cc93cdb8fb74f3e0e8267a49e491220a720c95d686b6a68bbaadd17078bdcb57" },
                { "bg", "0efdeec2db4fccbb11326d7cfaf9b81b2d6865b5e680e8d61c3b96192db104b1ed171c576a95afd59df4a4f87587dafecfcad2a4090476ddec498cf1d2ae40e6" },
                { "bn", "59089289bf4c713d113690d7632436888fca829808d3bd91c680d281f21d51d5fff63066a908998c4591a9f52fcb15436d498c330a6391d653612aced85d5eaf" },
                { "br", "be386094ec5c1e7efdc1465834f033dd55b3991d4c4820b046ef61e36f0369a375a638ef47f12ea00680b3c168561564f4b78c83f0d9514e9abb4ccacb96867b" },
                { "bs", "f5883c4f8eb253585a965246dc3631985765c7f798b4b1011fd49392403d8707fc7e4658f125366d545ff5df2cc3b0684186aeadf8191bf4580d34f84dd49fb1" },
                { "ca", "c4c7e17b526d1bbb9535d210a8eeeb3587e259f7ea8cbd38f7a31552291e5b75d46e6bc6414ed3ed5be3b296ed817c17bde9f698bc3a0b608e7d5fd28650f806" },
                { "cak", "5dfce715a9a7aa00920a359c9e6a6f33f24cbeaec4cb3993f0ee393bb3a06bc97cf6f037ec607d950691e51bf88e133e342b30d5b75f95842fe4e34c20520217" },
                { "cs", "002bd295e4348d10d599c0d6944b1a94f2f198e3d95d66f427828773762138684e189b1e5bf811aa188ea155ce9fe6a7167cd302b4040aa55fc1d4995f708905" },
                { "cy", "2d2ad970b906459ebd34fb16060671c25c2fdf76fc89faece6b1f8c22a2fc6e43f10a2af9747896ba0e1be72732cec3228abf3833218386276a39e9f5e36763e" },
                { "da", "40bea2f4475ec5fc84495b2ae7d5e2ce2e8167ea45cd0a5cefb2a3078e08ab9d6f4682fbed954df26e5355b4590ba49bc68747b0a7a49d6a89862b331a0b8d53" },
                { "de", "ead3fc9769d0808930b67ca043cddbfe8bcadd5b7bb278dc0563e45bca0492ede00bc6761f4e1543130ae75512b7ce8cbe0ec57fb38c222362c8fafb6b3f45e0" },
                { "dsb", "8e93c8286091980e3584bb1ed59b480ff3383dbb9a17f4d7f9178ef0edec6b3ba546e17e0567209889d7c8c31a4e99bf799822b6844ae3ce10eced9bf55c484b" },
                { "el", "03711d8e6df739f75b4f02519c38777f24f1f60584d76ceba1522b79240f50adeb2034f966c2f6d8bede943f124bfaaafb57d0ffad4136c4f89147bc7087457b" },
                { "en-CA", "f7d8ea4d27d881b0ca520f630c5e453932278199ac7607877399d4139fcb39ace4562f9c82d5152d38038c58e4b2e53b737b3836ec189046f8b6b551e881b796" },
                { "en-GB", "8aceddc902f872089c5ea48bc69af41b9074322411e244da3e2c692fdf44cfe51fcd9cbac913fe41f4ebc2d988829adb2d291f908340a14d06df36dcda09e576" },
                { "en-US", "723a4b53b923b141865c438ff54c230f049a85bb279ddd222b47a0f478cac35b3bf819938edcf50f560c7b970940b32c207cc2e962812ebed489d19df8024c27" },
                { "eo", "46ceb3452c024f97685b86e5540efe7ed3913c7b567f874f07964592dc47fab1eb38c6ee72164a68393e80c079c35c59e446961ea9cbbff22fc905109b6c0eaa" },
                { "es-AR", "83e20439633551e63b7fec70df99ce5cb410e3497485addef7359acdad922d6c8dfd979e58bb24926b3f1d9f84e234248f68f5e9a2e0008f42049a1d3a8e3370" },
                { "es-CL", "1dcba2cd415a8ed2ac87cd790f016fb0199d843551ba86d31afaad24b5e4e96f6f82cea3b8ef5e50ce0b0adc705d715417a211debd0c34c78976348e75ab25d1" },
                { "es-ES", "4ad5e75e20a9a16a41c8c2becb9d4137142e55fee3b2dc2480957aa620a0537ed3f5ff63d5d6f6be3e9494916b49fb5169a1fe7b886cba1c8ed19bf5536f1805" },
                { "es-MX", "844ba40668a5827dac5db75a5938d47448243848c27445da493f764695868817a6863c23bb5b20994a7a373ae74dc25ba9707d9b41f5a20a427e05479f73ed14" },
                { "et", "882d64e4f41f9de224060b5b74b2919ef9ef91848f70e47435b9ecb98d216fd2c7b27efc75570e7443b38c22109beb23c4a2d495a1ada484f848cc42b0025d21" },
                { "eu", "4ff86d7d61dfa7345ea1c43cf237bcb8ff6a583727b78f8db4cb246a28745f1dfa62c3f4b79f9ceb68c1057ac86bfebb4e6bb431035ed8a23d9ccc9280e0c616" },
                { "fa", "3268ea20667d4ad7e2654571ac399a1a7052fabdc1e5acd0f5cd39939d3a1a68e23fe177629b8611a9bf80b63cbfd328e7517a8845021e1c69e0472381f20f7e" },
                { "ff", "54829d27135a41ba100a61a4e95b5e638d44672d969671345a61ec63077af66be6c662d1ffe4502ac6c92d6f0a27b74f28ab4121c85a7e2e2ee15786ece03f93" },
                { "fi", "7e105229cfaa19afc0a369e0f080083538302b1a2502aa8b1592e40e094eb92b4e89b6cdd53359c343a2981b770d73dc1be4d64e50bd6daeead0af4fa20b89b3" },
                { "fr", "07c75daf7c784926571e0786057f9e492ebb873cb4605c68f916d2eb551257df99120bdc25c380de95d19e20532d3459e6a71297ac87d37adb8ce4e7e7e704ff" },
                { "fur", "673b4440ac0e0ebaa14961f6e4c7f7a00e85497ccdf503deaba48ce2512c3d5a32355d23c9977183c23de58098d110564b83ddeb86c542b932d2444454e507a4" },
                { "fy-NL", "ffa3d6922bcbca789ca20796ab439812a673afaead26984e7cdee1b711607abc42eb11a278c9930deeae45f281de31c5aa22a126584ac125057abb277bf55a4c" },
                { "ga-IE", "e5a7a83e8078a93b7406ebe4ae3a509ae8c5167da8f3074403c9fccd02b9ea326b4bce196497d2ed0c2f027a75ea656d68a76b091b54f60fe6d4dcf277c91685" },
                { "gd", "deed927afdc9bc7a0c58003bc431b306aace17399cac7caa05f3ad525759cd0adbf8030785b5fa1fc48aa441573f48c3e1d28acedfd1b73ba447092faebc302f" },
                { "gl", "7a2cd6901105a468f848b25c45994e4b54f3e8ea8530f2d5415bf7f28ffae2cdcd7851d0de76ba9853b16ecd55fd92f2d2c58d8fef5ca40899a76cb533ed9d1a" },
                { "gn", "e75058bf1bcb806a41aa9fafe7d3e36aa589f85ff8ba2ab9741225d4079dc2df7c5ce342ba3b346a80f2d8f197b3d4b28af26cd0bf218e60cf5a950ebcea54c0" },
                { "gu-IN", "b7add12130b58ad3b0171e1910d6f5db99314357edab5514ae0f6f4a95df3835a3ac2c295ddfe143aa139c9704e24699de12648513cde63756e915d333e6e034" },
                { "he", "6ee0cca27a417afd6451e372fef54e91319c3d54aeeca35f51313ff7978b4e156c1d0fb7a257032aec20c18746bb124bc4e88c730455cb123f8a95c5f074ed6a" },
                { "hi-IN", "a9ed48aa91dd8e7f1a6a536e4d3320f5343c259435b3f9b3cc1144b1f5fa2c77d210a929e8821c435f598cf7dd28d005939f94ef3e4394929c47caf7fafc4236" },
                { "hr", "0df28712ccc7eb8963092c11291fe00dc295dff3ab2c8544e5fe90271f409c4e5fc1616f1cbfece0a0edbd8ed7ec226c8123203e57ddc2b9b7b58d53baa988fd" },
                { "hsb", "4b6ba70e8dc9f3997839588eaddbafe961b87bf2f0e51b1520c10ac73207393b9d0363912c55ea50cea3d567566e4c3f99873a6c175e049feb34084928a94005" },
                { "hu", "cf40c127b9fbbb2948f7fab94c91be7941066bc72e1cde038bfcd757dc5d634ea1380ae2e79e6a52af1e5ebff6dab002dafe1ac4f4da8fb4a7ea8ebaaf379289" },
                { "hy-AM", "3b71bd114b066c83a52a66cb8cb64acb7e756a96e32ad7a0d108ba0b0f0cbea4c35bec18bd8b637d798acd2ef9e9668122dbb91e824b9fecf30f73a6824374e5" },
                { "ia", "4fbadfda1c6989a5e1ba4bd36c81ba343654bc8df0e709c3a698a9c0a6b3bf3e1178bab94dbfd50708565f6dfee8dbac52fe48466813a9be01dd766f0af9e722" },
                { "id", "d23a50f3a93d2b29cbb7fc409efd3dce368a1d4bf5991697dba8a49f425c3fb86b497704286f885fedf541acb2e6c1e15aa425b6338a7ba083d5ee814a71c535" },
                { "is", "1b59980b24dab5e58b0841f27f1cf1735093665cac2152c2fdd7859affae11327cc662e6dddc58d2577afeb052d6e0c2b9ca769a694e49d8696942fd9bb3ba21" },
                { "it", "54cdc407d5aabc1a4ddb56213786dec49f78526c7c968758daf44e8e42af75e41c94db04f1dee927b2f54493b69ada63c15c6f423ea80762c7e044165564712d" },
                { "ja", "534a2c42116aa4507c48615489ff9f84c2172386da4981c0129b8a646407b574e793630e65e0bcd4d1f7bdf2dde5e0d92f3145f954a9cbac9db45faedc38c25b" },
                { "ka", "ef654932fd650cc312d5038d04321c23f081585993b7e3fbf35e1198dd359e4d6fbb10edcbc3ae841e683ed8cebdc74437c1c77673ccb0e25cf272b1724344f6" },
                { "kab", "f425374c2b140876b7ee820f0d7cef68ce1c2ef1e10acca7cedac9c2d891795c15f4c26e76e0c02819973bb0dd024c2e17c4425a4a546184794474f57a1a03db" },
                { "kk", "d9ba717bfd7c65f1a7b2bf5d54e0071c0b6cec5062d056b776c1f11891454840ee706f39349dc45620189c747fac4fbbcd4bb632462c0fed9efdc8d99713402d" },
                { "km", "a3054da923ad342577b74bd691d3e06339742d7b3589d936028a27780b64c29ad3fa0e8ba89915bd66029b4b37864bca3e9fee108607b38ba93f8a49cd2d79e6" },
                { "kn", "ffb663cd2db7a96cc90f168a3c80c684727c9ad6c68987b1939ae65482e6189825acb0672397fc18f6a4f3b2f983e80504184c42ece46e470c5f78ce8b5da8ee" },
                { "ko", "b6b25465ab5dcd0d0bbf21972db705334f3dfa8dc10350c93e17185c3dd4911d2bf92fd975f34ce84a6b7345e8334942bad84a7a23f6731ddc0226670838c725" },
                { "lij", "44439012352d923cc3f9ce61c84f5dc44b02d6769a8c9bf4aa01dd15c0a55902dd2acde37e07ab1828fee6b7c0e568172b1a56a4c1ace80142acc9ecf39e0cfc" },
                { "lt", "634b2bf580fbad3b9c7027717cb752ed0bf89a1de6d485f48aba27c8b17a479daae3750c23fefa5a88b6c390b9f7b9241191d3c32dd0129d3126219261544328" },
                { "lv", "060a4cdab6b8a348187522e40c984f5dbdbac97dabceb14a004e959d041470bc4b91bfcd27e6ca7ade9a18eecc4216b43d789ed78b11210da04a004451012c37" },
                { "mk", "a13a2dd4b0fa95e14683829c812f8cec6dbfb33a8afa2a5701ccfc5a9663f6890d9c0139969fdc1eacadb4a0c467359d4fac19ac573a3416e9427d36acf1051d" },
                { "mr", "a26846bfccf8f9770925302e8d94ee254423e6b76a24d81dffa8fa464ed5a5e010d5a38e560964f52316880cc2c9ed81c23b047127ed1f9474cc186202518d75" },
                { "ms", "c6b00dc40e1770efd8617c2285701e3e92df5bc9dc05ee4b0ea6a0e4a4b0c439162930b6353c84ac47996d90c69d090151ac0c9cac355c8ec644abd5e9969676" },
                { "my", "e97f576713bd0479bd35d9b1f3321708ca56d733ca662e9a9bbf61b706c08903899b4d2b6d186359addf6c65127b97c57f5e1b27c7c691fe817522574f11742d" },
                { "nb-NO", "2c8e1e93e2c32fb7d7f9510aae1e2b41585ca1449ec8e780e867dcecc620dad438858ba5fa6043a2465193356960b363d72922328012daefdff8ad4ac15d8d1b" },
                { "ne-NP", "f4ab91cfe7cb18ca800c9fb789f0b06a476c85828718d9292ef7fe8116724d28ac53c351e0b5950f4469d82f518dbef1affde60f02c68346395fb003d38ca5f8" },
                { "nl", "a6630b974be0a8f243b57925b53c2eb071d132ba06b7479ad1702a1f1c0ad69918505cf858ddd5d1522e8d92e499cb35275643f13c940d1b73ba81ad726444c8" },
                { "nn-NO", "2c82ca04b26c211511a1b587653c208f11a6a665586db7b089628819b0a39aa1c2e2247e3a9af605ba7fcd8b4b968a6fd93c223aab1007d225a028b0dcdf3792" },
                { "oc", "0840c75062b5f5f601e81d167071269b84e62977621e2e3cac56b44ae15828985224af644df84cd2c4248133b6266af65ac55ce4de1650a1a2fd02997c0206df" },
                { "pa-IN", "1a5e58f791400a41ed6d27a35e0893486d2bdae0010073c06ee0f68305d24f08ec42ef3cd16d0fd0eb6e3b39d8cf7dcd30d2bbc7dee7f8e2c7b7d0ba424ba1dc" },
                { "pl", "0e8752d8e51f5a71e7c62312f2e6fc30c92145642d12e9ba7c07160ee7edcccae263937a344f700204985fbb3a732b045880b81006aa979d73d301caf653c901" },
                { "pt-BR", "568a3cdcef4d5adcf2cb5d5ce6ec6cf6a8f4a2c9ab19364dccdda460e513599d3ff1f28a0158059f62b4e9cfb9af2391c25aa1f2092f0699d3d062da8a499c22" },
                { "pt-PT", "d265aa6652ea66f06479140bafcf3056b7fa2b8f815b842742b734fd4b46812502ef16727ed7d162e52699318f0c3181a2a0875ddb9d61fd98d0e3b9e11a2a62" },
                { "rm", "6bc60939f6cfbf94d79443bbacd98dd2e77e464a71389e8c0defc3f975208161877336dc1ccd95a6055f967fccb076afe9b761f55d779e63573c6085957f0a34" },
                { "ro", "1ee743efd1f345ae9f89ef91755edaa6f05c5b1b440d71568c28e8dcabfcd5a37428055b9feb640746af551488d8db72fb6d977118aee5469fb5438d8a6ad4a5" },
                { "ru", "07bc6a9587816afbe0263abfae44356d9ec5745d7f8a6d4545e0abb5024c31b888670560ca116d72d1d17d3c15bc9a315a2f238a7f1067782f51d40343e1bc6f" },
                { "sat", "266da9b52369e7ed3893485e5add1aca944f69ad54dadab1cc249039afd1cd993d535cea4af2cd98a526653d4773d0e9ae03d99ae3d78972175896f9cf2c377c" },
                { "sc", "df1047b6113091fa460a2a6d1779a3cd4e6fc0e60280345f600836c9ef57365b076f7233efca645540397346bda4b6d9bad82b15deb33fc6a22ef205d933ef0d" },
                { "sco", "c56a0350704b5bae01a8c67023b3c1d136e89ef25129868e14d0a2be35c0ffaa9e23e4b2dda2c1d67e43a6b7dd4ca8308b14226f8189738377570b0e566b7a1e" },
                { "si", "e9da3069a0814206ece4d7844eb8e6db89dbdea929446fc438a9aade44d3412f55a01bad55a98079c29b7938ae7deb374788b9030bd3ba77ddf6a2d59d2ddb51" },
                { "sk", "ac0fc564c89e8642a7b24cd4cd75d420650f06c078c5750fcd1e716fb0a352abef095ab968caa720d28d220d549f0000822c8343ce0477ea82b0a26ab9aa32c0" },
                { "skr", "9e108a5aac671ebfead7c9ce74b1380aadf1596b3fdc5922e63a3cd7ba659e17bc47d01f71cea10b22bc9a2281a39d34792c40d7b2ea299bfc6eb87521f63ea8" },
                { "sl", "625dc4061561d0db118ea91d29de6b5c2579817cb918c0e22e8f00c22f63a792f3498eccd5603d2773756bf3f7c35222d504cc3550474d94715ea122b1f8c815" },
                { "son", "2891b0ab346079f158b186abd640fcd7fc7e4e69e32d7d7223d28490d8aa13f10aaaecd1c98a9c206f0dae2d1423925a05001aa34b0807c8d9806a278085cac9" },
                { "sq", "8b72ef723c2002e2abc84df836a6fe45a4e84ac16948eb22e69f4117ddd8c4fea699908392176673a9a32abcf2a8182625bd9c4d228b817e26c6fc9ae03f1536" },
                { "sr", "d1b66009c111b2e7598ee1c3119651b38431742ab02f8955c35f638eb2bd647f95e66d1d77b81dea67db8f79e0f5d9015dcfee7b98909733b12901057ad00bce" },
                { "sv-SE", "6a867c367260c107c21250e2fd01b09d33443de98908278f2271ae1b8b52d5ff152304b5e1b4bc959fa079dabd15ea8a759536dc3506d2257310ec31dada6c14" },
                { "szl", "4973a6b2451de372c9ca599701016a3d24e11cd77b036408345c63a4d1847e3820977c5ad02fb31a75047e2fa0f7007e99a7229b611e2b0f004e85f522e9e257" },
                { "ta", "c85d8ef670322d6cf6180df8ffa8f58c2268978b800577166180a7e766b7e7e9efa1b8c4ee26ef6b48f8def8dc7cd0823eef86801f33555f6179437d6e6735ae" },
                { "te", "4cb031ade7da0ca667337caf39dbf3e50c480813b5e579ea17915b3881e5ee3e17f1e5dbd12889d1505af7666a699e268880796cdf86fc527cd2fe8f7269dd0a" },
                { "tg", "d597aac9e60414d488e88474cf6fbda96f49040c791d32baa88e0db3e10e98a5c60ceee5385544b568851ba4eb16b6c6ad6fe3a573b774975a0e03a9830c72dd" },
                { "th", "f811093ead6ce80870aaa7ed7235213ba479b957fa8e19f279ce1065c4ad77b2b2b6af7083f18490b9af25bc509af5e8a13d2bcaab7d08c6703dfb161c4878db" },
                { "tl", "e0e7ac0ea95b31e066d09c1017ae5a97b1fa471c5a0d52f6077065354b83af7dbdbfa259c7e2d468d27611896ed60925206430c774b5bda2785c59279ea2200e" },
                { "tr", "d8742daa095dd3d0cd61cea367f8670bb66d838407a21c1547930718d943a8cbb8bd5eb8ed23820210cc033dc317d84313b0665e22030f73e722e8a381122367" },
                { "trs", "fa21e03980bd17fcc00fab6af72521898140b8d1c333c3ea665778b5cd6afb3f707e0a71f15985a972ff783535998ad694ccc7beda94db9a01a08611c01ac4b0" },
                { "uk", "9390fc48d8df1786bac33f9f58f2ae5925ae4b73d6961bfbeec79a07082cd952996f55280c34fc5829abc825700c217bed95468583e6c39d8d55036a93bc3633" },
                { "ur", "9ee060dd08f8afe90b190d2e965a63be2a2bac1dd63d22927a1d1502ee07b526b16e0c9b024902bd1bde07c2a0191bd44fa04a88d6bff7dc9db2f646b4866468" },
                { "uz", "b0efd881a855cf9c3863d804e6b82c9b609d7ca11b3a58c3fe516a6be0ba90d37410e2ffdb1872871f59f0d9167bbcf9eae9069f147557d11843dfe8cc950fbb" },
                { "vi", "8c66102ee389b57e6e71bb02ef7d1d2531a4ceadaf68a06b88ca682bdc9288ea49c21c7f0d2cdd0f5ea338ee3cf2972638fbe3901cb38f836a4fb022b02df907" },
                { "xh", "1403383af44229a258ba7cab22f895b488097e6b1a192bd6029cb497022d2a6802945800e3a770d7a1a3818959389de49cb36906f3a38009c2ae0eb854a356cf" },
                { "zh-CN", "c8c298cd4a4745259f7fb62ed2aa8c0073c614b291970a6e3bc2982024a5520d5204fc512271f3bc8c7ea9986d52bbbba212def5ff29d8b5b528586b95adae6d" },
                { "zh-TW", "1eb576fb210713ad4dd422816f5b173b5ec7e389b77492fdd13caa425ea6374d4697ec754a02a5288bf940ddedce856877d3843217904d3758b4ae11edbb2569" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
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
                return versions[^1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
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
        /// Determines whether the method searchForNewer() is implemented.
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
