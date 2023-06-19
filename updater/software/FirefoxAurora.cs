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
        private const string currentVersion = "115.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b7/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "c7b61a2e7c3877a8232f8fcd5172db6d25bfc762e9d4d9bf3cfd56e897be0e512fd0754800bea7dfe9219fcab053c0d744dcaa4fd6ecc324b73afd41e12eb32a" },
                { "af", "718efeab0e786016d288bd6beab242cdfc3721b6a99f01826d246cd773c32641da93800096cecd32996f295d4e081d8fc553e1861b199e77962b86c395f9aa00" },
                { "an", "3ebb89270c10cefe4fd860b958e2aa6799f17a31cc05cd809928b77cc8ad8c7d56d785b4cbfbc30a351f438e0d9f31de40c71f02ebea856ad9549d887660156c" },
                { "ar", "b28a49449ee63be3cdce08e8ed8ec68e2ce9b4cebc68e15cbf276101f9f5672568191e575f3e53ff3281ee893bf2c0a618cdf5653fb01a81c925d4f94b297f20" },
                { "ast", "3b242b4937c5704d2db2697e7c8f1d2420ca2e4fbaeec22291357bb2f83dd6fab81e22e7b921dc290416ef44aab020205b03f7eeb77de2a3a69b6d51df5e407a" },
                { "az", "88ffb82ba26bc30f9ca48ba73bb9313bdf71babd9301c3e8b6506a6f3c4bb51d207c6ded415b93845bfbfc943d205d5c95b40c52d1ef9d90493bf62075c799df" },
                { "be", "cd61c2bb98df47380a14f86511fcd51890c0086632a08c598cd1bb755b4a2cb6314edd3678d8796cab74781735641bb849b529cddb58c993fc79db2f3bc4adac" },
                { "bg", "528d3a3c0a2ff97a380d7ad0375e56231bf0b378b9dc557277f82a1996c51cbd33d6b85fb328029a7f89376a755994213d6161b441d14c0c7f274f48550a122c" },
                { "bn", "3b7c534f5e34e9cf6431144441c23ab03db2a2f3656369732a78418e63ec772ea238d99c313c3c71ca9c3bd48b053172da8263e184b34b98ce1a49389d16eb5a" },
                { "br", "7e88943c4d8fff2ec7d28ce7cb91be409901ae951b19bfccdf23244bdfde16f90313bf2b0b2ddea07079ebca445cf9a7497fe6d3160629047915ce1453dc4787" },
                { "bs", "1b14fc81b2231d24e92ae822354ddedd23dd6eaa9e9f2f1a2e9ed19ffd80b2606a4c8697e8027bde98cc75a17cfa8337459c9436998345a76c57c5efb16bea9e" },
                { "ca", "0ef2ecc141f1f8d33c2c6c8c0e2a04110cb684e0332cc250031c7250a17620cfd3284fcf000746fb29a9b2a180d4047544a820111532b2084f1f562b5ea0b27b" },
                { "cak", "c910fe34b135aeb5ce4cf266ff7bd2e03c5d5c8b793cce26eee414b1042899c69fe0a8550780d9ae4ec5523f1e36c373e602c1c442e1c3632d3b35cd19c876be" },
                { "cs", "1b697a27b545d3b664117e382531272d0dd871d88e1094fcb17c5ab12e3576af4f428966020f61812015e8e067588852c6e0d7f98c4cd3d49d6d90a1b40e31c6" },
                { "cy", "121abe05fb847e3cb759199c19eb647f68d89d0d6ef055b6125185831bf0c624c23eaee79c2f544ee04e913e2c9f50130cac6a74c690f21c90d6e8ccbb621a5f" },
                { "da", "ff444a62cff14980ac4ccaeda676077578e4661a2985623a2bef9564403e6f86fd53df0e80bec1e80a3b615b0df8c2c478fa346520c907cf578f6314b4efc11c" },
                { "de", "aaae20429fa9d4b0c9b2581248b71cc86de6f438c29e9f3f35ecf7ba5939fb478364cfaf6ca9515b99ee57b21cc963af994cf8e8a5eb4ef55da120def168e4ab" },
                { "dsb", "51d12be3818f3caeef0cc9a5d328a44e6204c62903c4333f5ac568f5ea219a5ddb925e3f9a1c9076a48010e3e1062e5d21a779211db5318f02fa2cb110b6595e" },
                { "el", "3a827dbaa5f5e861e38fd90bb2e04f05135b5b9bb58edf632ea542d789fc891b3fd7f947c5ae84ceae89f88dadc8c99079cd76b6d714dae3b49fb3fb3e9b8a24" },
                { "en-CA", "5151e266cd0186a7844fbcd57070fe50158368a6f901baa548058b75bfcb58eac7660ae486ee79e6055c54061cacbeaee1551c0a6fac4e6da6bcf50ea47bf274" },
                { "en-GB", "5c99f9e880539f376b661804678f4a40fda9accaaa7047e9a2aab3e167e2330841eed4ce8b11e860efaf28ff9deea2e4ce11637b8c5c661e0791dae7109bc595" },
                { "en-US", "b4a08137961fb839b6e67e10c13ea819e6b9d30fbc5d2b353dc4bb3266e0e66e7f70f05f3dbc5a5eab70b513a690094b6ddf1fa2f04ce8ec610360cce439e0b7" },
                { "eo", "441a94a25bc264b5d807965e0f0b26d4e911ac0ed8aa3cc490ca40d5e770bfff8fbf4c6425a242ccd99c40ba72465a95732cd42d7e3ee743abcab4471466dbc3" },
                { "es-AR", "b75e838b7b200643fe40731b291582905183b397d6e3c571dc9720fd8ec86e420571c339b2857782a64eb408008b3c097684ce94d11c12a6eab6c6afec115398" },
                { "es-CL", "d10db7fe8a4fc966cd821f1738ca8e98aba4bd0823dea718fd1d088c7c94eda4ce10717859830d97bea3b138ccf216a361420df3a29855ebcaa09f12d887abea" },
                { "es-ES", "e8ddb87997d55b4fe6e1716714b6359fb41a0ee196f706d39caa1f30d0a68b5b58a373bad5ef32eb5b53d1f206db5f4fbb79f046e8df3db62b43eb8d4f4ea7a3" },
                { "es-MX", "e9083546d3149d7f6ca95da5b72bb468376aabda7f28c7ae56274a6d51660d687a3fb3a60eb52178e48c15503a09a3e852915326607fb0996db77c8cd3b76e72" },
                { "et", "5aee94a4b560183eb0177e8986e6367ceabeab2ae9deeffe64610423baba660877bd1dabfc182faac4b8b9bdf694305c3df8fb1e02afb9ee6a3cca3295221359" },
                { "eu", "a33f70e21a5922b4624447a590c11050b889367eeff292a659bb7cdcfee15d39c65adc5dd95deaa22e9a2b80563c32829867e4e4a04487ea4bb0de780c5eceb3" },
                { "fa", "046217a8dc493fb1bd192845df8ee9e8576bbe8cd0b33e2aaf225ada4b055798128892b6491b62b1a3e0991bbb046b7fb8bcc092d69b93fd5ca033b4afa92655" },
                { "ff", "7a4257302d59a98db078bf98665d78f20c7af67bd95a5122329d47a466b85bfcb3d00c130159434b91bbc98db37f2f6a22154d032c49c3d620e7cf73cc80ef51" },
                { "fi", "073d9f3db48d683c95e9c8a5ed3362c2dadc50b7ee605e67e2ff97c380c22fd95739ea5448cce0976fbdcea57633b8db4cf2e1b76ad2179ff16397bb1433c33e" },
                { "fr", "c5d3677580de5cbf3633aebbeadd73529a58d7c07749a6b6d95429465ec64cf885c9ad5eb70a18053ba30372306ab260e3be3fc22053a00bd0ab98c347961023" },
                { "fur", "1cde5714cc53af4ceace914387f7d59ecad31fe1c0942d6821ee14162be4fa207f9e8c9512d75eae99100cbb35c222bb3981798966ec7a00ca9a79cbb4111cf6" },
                { "fy-NL", "d3f2105e550f072388bcfc41e60da9b4b590fd439318f38f9164f20ed8512aec94ca0dc5b01c7cc32233b3dfdbd7a1c686eec9c12d7cecf085ac3366e14cd351" },
                { "ga-IE", "64189b6d70591c65a6b20237fc64358d34e46fa91f836d2cf4f52b1b3ebabacfec3a366d909139c370cd919fdb865309ae3409bb213e93fcafe2f58038eb58ea" },
                { "gd", "00c74a5dd0eb197b17aa5129e87fbc7d41376a9698eb75ef9238e07b347fc388e4f41cb71ab775c4e9ea3354a223e7fab8a985f8cda36387a5b3277d2635a82b" },
                { "gl", "61130c8a2dbefdf6ee3b02c7095e3ab45c5b244b1acdff08000f8846f0979bd0b75ab462493f8ff6c25fa790701416f60cda830dc5d0b7c73045fc005369bdd1" },
                { "gn", "776501f5f191bd98bc8c67a04638ff1b9325eb16e3e94c54f9b38efd7c662b74a381eb8a69589a86a9740c0b1c12cec99e0aa56bb5ff980dc525bb7c45540e30" },
                { "gu-IN", "3ef6fb138f899d0e720caaf1035cfa5c63ac81fbb9b2eb7ed92b9a24744254a97c1852f3132c7874272959d2154e25e264ffbad1d73c92b510c700f9d226e045" },
                { "he", "f980bad9925b145bf12c4b12e85cfffbb358c0725e94140d1c8f9fbb8a570221875235be6f8927ef750c10399708bb598ddd22770ebb0ae40555cc46ec0fc8f7" },
                { "hi-IN", "989687c7ed7beebda07e7b4b46fca6e7f0fdd005380ed45d54e6bb485029afe4f1e6ac3c6c78ca82331fd8cd6f3d87f64ed78d1b1c3899c3d053353faeba55e0" },
                { "hr", "d8f68a3d82fdb36bcaa620403eae3a97b84b76fb959f64e4adc180b745b6b61e6c70b7156c3022382916f41824eb10bcb2eed9ab5706e1be8990c74472e0dc18" },
                { "hsb", "3052067a6e67bcc046d47ffab3fc8584771dbb99e173466282dbe0daa2629387f701d2656c75b83638f29cc83771777993151b8adc30178c23135c48591eb7d9" },
                { "hu", "12cf1cca8ccbbca6cce53d6c675c02c273f0e815cc13a6bab8418ee9f5b8630a65c37a20c7d91c34d09264539a4b822388cd93324a7fb143c109c4696956566f" },
                { "hy-AM", "0fe99709fb1208acb316fa47aa7ac1decb11fb83cd29cb66c25cfa58b2e22cf67ae2ac22b6b456555ff2b5316ce6f658cc94e41df83db1108f9a8f647d389ee8" },
                { "ia", "e3c22bf4bc05b2fa9a534eb50c74d48242ec5ed16e9b4d803c4ea81b1df5662f52c6859e2a625a33e1a3c5a8a3741eab2ec4cde289929fd4d9f76cfce220e399" },
                { "id", "cb10a7951960a531359ba6380fc46e2f8214c60c59151e8674444026cee3b58fb8349c343a5dd5d482ceae9035d2bc7abfe8965b2644794ad734757d6ef58c15" },
                { "is", "f86d32e8363fa64330d4069d7caff2a61b6a8058a514fd6afd197b8d2c97095af75bacc455d03c336b889797535cd0cbc842554313200bf50be2e7886efb5cd4" },
                { "it", "e403a2aa66ad83efb7d2c80fe1a7787aaf16ac0320e7ba0db38bf52e011d6f54d98a56b711462cf700a08949a80a793871c967134c08609c5b9077aababf9243" },
                { "ja", "1166e22d9347b23f5368bbd326691837d3acf14d0b269b959bb1d95985c120f5fd9f0711a0cefe4a48c15817b777c652c76434684f0ab66f912ca00bf8db4fe8" },
                { "ka", "21929fdf31d52222a01aaf49e43c6c35c8ada4731b144ff2d79b8a1d35709ef85c3e9b8ce0f66336bbf61d2a6e3df6d3f7beffbb3074d5cdd6a3b1439db81f56" },
                { "kab", "0ccd228880324fdf47733fface53cb143baeb2ad76584e5baf841a71cd9fad268d4897eb65fa0adf7b58b4ce9cee15fda8caea0edc7694bab8ebe42571836b68" },
                { "kk", "89e29dba3fec584b840a275748f2f5158358ebcf9390bd700cbdec8a0e7f96b4f19873d163988da047627564123ad015e3fe3034c4a203ff33515c1280330511" },
                { "km", "b104a3c70bdd8a27c9989962a55aa9985d64d0b4a08703d9aa57bb271aa1214ed9c07974f3471c070ec6c6c078af679ba7d63a3f012a6e4da50ab7e4be37c07a" },
                { "kn", "a05aeab82e943caa504e2eed46532c8cd8457da4c30412d454da0af0341d6a497eb5f7bb665d25acc1e8c749e8500176529df1db2afed92eec6b03d526edf7d9" },
                { "ko", "4659b5aa43b9bd431c9364057d065187cb49eb1fb370ebd637516b1f75359cbc354e1cc147b91fcfdad4c10a3c76adc89f013fcf038015c55dce2138f63c3382" },
                { "lij", "3ca51087ca22ae1786271b5793a1dabf272c7ff8d21c6fb9e15de748bfefc76b68c7adf4eec814e7681901ad3eb21d2ea64e4bef791111bd758538ce407a010a" },
                { "lt", "96f37f50f6cbc71272389c06652bb64fec9d6723306c117849c57f56d60bf26397741b8bce9d42b550bc1572c779b57d761f130cb5d03335af49872e2feae38c" },
                { "lv", "8a83c969db2a0051ece7f862337099519536f9d5d8f3c7d765be55c4aac25900834979e1a58ebe143d09ff9b20c9bb435cfbf7111a7117ecc82b1fd555ece0b7" },
                { "mk", "31a5ddb89169cfd795156ea9aefc421f310cf124bcb3a0c8f2308e78ca5d726443971f063b4b269b320ea34d8ceea953ea5d2a07b7d94c5eac4d51122b78594a" },
                { "mr", "6119611239f6f399ac487f6d2b95420c9c604a66a53f737e8b09d97afaff8ab750246c8a3607758c7aec62c11cf7df5ed3e1cea6a3b6aaa10b4186c6075e685f" },
                { "ms", "6ade4bd6c9b603327b02502e007e609d096500c17e8d0d4736d8ca00dce347deb0e6fb879725ff18e536dbe9ca081f96b0273554ff272ba6bcfd0fc2f0995771" },
                { "my", "5de982fea3101d4bf73284d3f4e83daab40b97bc1e0876484dc027613bb9ffb3a0c2484d59f01749ed001fe4b66684fa5dac0c7abda747ba98c3f022e6121805" },
                { "nb-NO", "200ad6af77c9c2b72563e4cc41cc9cb28852aca278a331c511bc499146285d9951f51921ebcb5e0878929cb4e44f282beb78ab59ac51978b57b7a12055932010" },
                { "ne-NP", "32ba20585d6670431815c2ddd5c0b4406cf98e36d9deba51ac68d1029f48a0ad558da9641c8d6c55491fef21c3f7d5663528b6f9948592b9cbe2e035ac2ba872" },
                { "nl", "cb0e3ea13bec1ad65d4aed4d50224a0e433cefa0dcc167f37084f6d2b5663bab007c1006ebe896b897057cbfac8e23a5e487d93e8008fd37dfe7354606435f46" },
                { "nn-NO", "803bd2a3aee0d309bcb1ba86dd0b9f5a671cc42a29a9ced8f748302e5a9b65a260308e9217b2d836ffa866f58ac9db70ee9a2dc20faf6dbe67b644c4990632c3" },
                { "oc", "c9ddddc469dbff54abc10b2d6eaf26a3c9cc299665d138a8039765a5f71833cf079bbaeecf5add5d1e968ecaa8eccba25811594d553d4bd05648ba082af1b3a9" },
                { "pa-IN", "9782714359af2b5c287bd500ac091f3961ed7a22845e2137245367c58ddddda4678af960f69917434b4d1c9ab453f73ed9fe7b909e31e3101dbd1840b45ec2ed" },
                { "pl", "bc1288ff8c48c6a7b9e07217a4f5dcc2d3ef2b8a54bc9fff524472cf6f507c38f0a41fb530317eea1dcb05c9a9860f5bbe14529159f1c7119614faa77199808e" },
                { "pt-BR", "87bfd3a71442da41b04cdf13db432667b6a689c39f0f56ecb7242aa8c23dda4631d252d9d8c9c18803c84da103f2503f4dc56137e1bc0489815182613e21931a" },
                { "pt-PT", "f49042f6bcbf42c316fa0f5b291fc6a189ebccf20b9fc8123e09b8009d3c94b2a775fd6388d6896b32efa5ea39c7ff5ebf9416031dd580d421737f2a49d9d607" },
                { "rm", "8313e54691c03673a162a5b670e743d278d5d724150de6a470d4bd4fff9366f9df29a90bf4ff083291d0444213ab5657ea876c53283176241fa84760c945c823" },
                { "ro", "ba036bf053cf096e99ec5577e79f4bdd48bc5c647cf5629ac98bfb83f5254653bcf0a1ab1c5dabfa73c2d35031a9628144be26090ce9fb5df99172d7f96ede14" },
                { "ru", "7f91a7dfe22cd7e238dd88eb828e01fd3f3d39a1585feec80571806329f77e92a03b9c4ed135eb17cb1fb7a1405e648727e617b27e18372fe15c0aada7446229" },
                { "sc", "8d631cfbab4d3ec1d48e2c63f969213a06616f0ac6255ba79b5f68c8dda8abb5beba53414cb7c31b05dd9f8e0797ef89366ecda2ca8bcc9361f8f9d9226491eb" },
                { "sco", "5e938ee363e40360a6b250a98a9c49ca467f5f47475201e3d439609cf30d8c9b3391d73487a0a5a3f87958aaaef618911c35b5776961fbea06d55423ab821378" },
                { "si", "26546e05a52b186c33bd83756c43129541e1584c3e48a954e84a725604e14e93f0130588193ff2d92824fa5d800d5cf89240f95317b1e0a76486551ddbef8882" },
                { "sk", "acc0628a83f3a59b7c8a5431b09f887fc5e013c107ed1a63b407e0f2126cfc926a9d5db770c0140d2bb13d1b5a45c230d82276dad912d67be386b097756ead0b" },
                { "sl", "648357d222f81c9351e939158cfb831dea8c1497d032eb78cbed833a6079e1c1bf9bafa20b41a3289a57cbbacdbb7da1ce0701b8badf22ec5c0139cb65ebd2e2" },
                { "son", "0e819fe89bf0c3aedfb10af8acd6581591febeb47e501d665f17ff743f2c1e044831b5e4779d4b84d1493d6a9c5dc07f36dded53a825f007e0e6e631e351bcee" },
                { "sq", "5a19516b90b65cb6ae75ebeca759237314cae173d80d4db3f65c242ff2999563ae53f073396425558b5168dee7d1b74ce01c9a3a480ddcb4dfeff8d35f6f3511" },
                { "sr", "5945e3e3ea6f2c23a747692714f8b229e0f89b75c9330dd39267b5c25abb28c9d31338c6f6fa3443752266cc52012352271ad8cc158e34fd37efe63f7e8b1603" },
                { "sv-SE", "93c99c21c20814dd72dff30e971b223c737c54390ee78b493fc253dfc31c320124c208becc9f9804f97fea730ed4e49a340e4873739fa593e211c3c4d45d6ee4" },
                { "szl", "6546346484b29e9f22597058043c5b2da04d2edda6426296778c0754c55836af4f0815f469e01b8fa0c0b4be116af69a3e2f6fdc6255134cb8021868ab1aac32" },
                { "ta", "1b040b9a33ca51be85ca77e2b26152b25e9e64d91fdcc94cdfb66c3ab0f38e02bd5ae7a44f0cbb50c48732cf837548eedc7b414be2679e54cbc961ee478618e6" },
                { "te", "77179f492a78077f7bc3d7b83f142f7901a57cda9b554372a30d93b048778f639164a79be2c3fcc58d155b34b61982e548bea3a97bf0a7644548186981bee556" },
                { "tg", "2b056fcf28fdbe451b0a20514723c6dbbf58c53b06e6553fabceaa79648c96a094570d0d428cfc59060d5af7a244d78d75dc531d540963e8dc412e0b5fe561a9" },
                { "th", "adc660099438ceb3200bde234951f1138fbef404dcffe4693790d66dce65e75b3a74f03aea5058ed69b84fb1219ffd48936736a20fc4de54841532f0659b2915" },
                { "tl", "86c0077c28a9e1f6ed8da615403ab4259ab23834ac40b35159192d31f33ea9d2e91cd944548f0a01a66a11d088c20869c8a4e05dcaa4be353e49112fadba012b" },
                { "tr", "c015ae7203fb4ade1ba68fb88da7e5daa9731443f39feb5387be951fd30ed0631278dd8cfe0934daf2a985830efd7d45f5a7c245b4e26229e76319bdaf8cea87" },
                { "trs", "0f10a0cfae01687108417a383ad7032fe8d1133d0b5ef2c74cf7cdd010eec4ac696692c42b4e8a507d80444b92b4b50545ea1a9c6699654c5d4bde5bd1620b57" },
                { "uk", "1db67b8cbe194f4d74e4e07631bbed6456cad230227faa82d0c2dbbff91812d2d849bec4ac638ec900bdb1e7e214d955ab5cadc9d1cf4b0ccb2b373529e37cfb" },
                { "ur", "e701c32ae862f4c46b910806041bd6f48600a194bd7df66867954947653814b2ec7175149c0738c04f414fb53c37ab41298074e1f4fbf219164ffce9df5955ca" },
                { "uz", "b91a8f26831bfd7ec40e939a9defa45a2be7984a86874178427b3c5fd36de7732963567791480732c411ea6fc4b207faf49a18eee29257f78ba8aaf5b492615d" },
                { "vi", "171092d835a296186ed0758742bdacfab84f2cbf8cb1f8bd4bc9e2b8c305e58f24ce9161e6d0b1c05457e41fc076509a2a106c0d086abb30af8facc8e69a2222" },
                { "xh", "d2f3e65b2d694ee660d076bb3a9876984c187d9ca4e2893c86e736e964d685a5d6c13db4ed87f1c6225cb33ab6f4e8bc9128a686fb17c0a3c8c744a1092181c2" },
                { "zh-CN", "622f4b5b43526ee7d1b886075c12ce8503e471a4b2f8d9b310129f7f720e4304fcc61fded817edc2a14205145722598947c629eb53f3fb8c71c2a9ea3f58ef9d" },
                { "zh-TW", "d827f7457db4edf32012079f0b40102ce213a398ff160b68ce30fa5a309ae19dfe1c2d13993551531498719ca9f0d0a3e799601a700b897d867e514586752725" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b7/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "a6604ac100c889344707f6eed2b551f27c6f252911caf3f3ea7cbc9eeef9bd3aff56fe69609a8de0e8a583f7cf24d045480a17254a2c3477eef3ff3e23e00eb4" },
                { "af", "bd8e04941ffce9fc384df26e0c5e42537cbc662ec761c121330b338719ef9be41214b2bf3c8ccba2f787ea461beab7e465fe280c4e6a7c21061859c97e49bb12" },
                { "an", "bf0c28710c884157941857d85adef65668b58cfc0e2ad5e9720145c2c6fce519f67086bb5eb939aba9623eef4b7c8df31ec7108b08c68325326cf9f66ae50092" },
                { "ar", "4a5ecf825e9dbd6f9e6e56f891ed4bddc4c24ca6635106ef0146470d0d9012c940a66ee3a4097a28cce5be1be8fa6e3bc643b2a648df38403a8e57adb4649eec" },
                { "ast", "8b4e0c29e1c6b9a8c4bcc188ba5f9025b44e668d8f5a225a41dcffc910cadf3ed644ed7c6edb8080688d53e739fde074ba695016109af8ecab0bac13aa77b9fd" },
                { "az", "588b8a91b02911fd03eb10e4ab5ae474d8648c1cbb7443da743ebc62f9426ba48f595349da8bac5c9669ce0904eb3ff612596710215790a516c0dcc881c0787c" },
                { "be", "423935ce9858d3d31ada62d41a9035f3efc52d5a50604e964111f5a18b89a6cae97d9a115fd67765b591a6c2d200c1eb0f58ead3390338d5d4e116ff29512075" },
                { "bg", "a60747ac5b7aebf85d8e3269ce7bc84f88c94dd39515e1b693a23991b6429be24f877a1136ecb90b1ad25c35ee50458c2770f2952eb303eb05508cbd61d16f64" },
                { "bn", "abe080f21f63e4160c2c272c2ad9c6c5454b3ae4f86fe5668061bb075630adb7d5db0d0a9ebd9e9fcd3bb80bcfd7de3691cb1d576efeff50fd5a549778b47dba" },
                { "br", "ea5e4565e09bde70a730fe34496f91d5faccc726193881a83b2bbc6a74d475a6f5e7cfb185c7f2aa941f062774a4eca34de4d1c6601289f11f09450eb6d97e02" },
                { "bs", "e5835c4727172da2898225022be08e75b8bd8b9022dd5f36a6f9df1816c3d98857370d0d8be6c642d4bb79d2fcf8f350da306f4896f8f089b7ca81a7a0734d31" },
                { "ca", "db22df366d6b745ee68b7f3efc3f53104d0c923cc2a098379cda45736360e5625023fa0b56e97f1932f2c07ce26efdd495020d2df7179aa0da720bc98f729b12" },
                { "cak", "f49933e28cb91f52db37cf662c4ee72b5d38293edec83efae61dc1a7cd4aadf9b38bca4399cd7c97f3fac028f6a80d866413386ded4c920d560a10221b82704b" },
                { "cs", "b1af8b5435a4749cdfec4478a2585100b27edff579dc9cdf1e96ac34fc1bb87c0ba62a47547e1aa0afc99b4363187a243e0a8f9ce5f97fd095ff648fb5ffbca8" },
                { "cy", "ff1dc89332a64e210cd435dda48626ce5db67f08b8b7332e0d4d9f93ebe1ba8c62c61aeea6edd915fdd4c8fa31ef3b96a1f23adf5d9f0c494ed3d1dbd8545dfd" },
                { "da", "16388baec0f3833e582a846d9d9155d08d2a3740f2902d027421e7dc0cdac173506be080f88be933b13548d1b8ce75756a9291deb1718452e22357b1a7bfb834" },
                { "de", "3544a705e3186553135ee95b1c74d778d4da73f3c77892bb655de7f13ebf6d4fa20ccac996eb555cd8a90dd8f67476818aebe2ce08a147a8f999122f5bd65fcd" },
                { "dsb", "729876ae43e4272f2a6e363562fe3d291bb7fccbd8176b5fff33ffa9ae2fbf968177e946bfd370af4987e4cb7303e3b86bd439103010b7c510877ee21230773e" },
                { "el", "8eabcedb0de54f628ba703686be65ccbd5dcf88ab01e03d83d844aaa2dce3374fed2df7124a027e165fcbae985b24d4c406aec4ce444d96e304d380a500cec7a" },
                { "en-CA", "96f4f92211d0dcf25fc98c29902df44e8e39f5b49b4dfb5e26befada90f8da7b9633e702b9a26d36ec81034ecdb8a5644e3b7a6613d828edc2a1e09961224810" },
                { "en-GB", "10621dd25bacf83ae7f5da6669131a42d3d0bb12b70d476fcee8ee004834719d5a0ad0e19fc8b4d3d90dac63dde58f3ffc3561fcec902a6469177fd663122948" },
                { "en-US", "cfcbb84bed993316be763d06e68895edb44ecbee035572fc3f0bac444bacbe4bbd6cc4bd834489b8de6e0388bde531c02e46c495c1a62c4f2b1c95ac6ce02d17" },
                { "eo", "7d3f7bb324dd2ad0a697ae60f6a8cfc4855ad1f93ef2c61e85704b988a26a2778ebb07eb1e9b2685f7d558c1fa9f98486a2303b20cc2858ce97aa5d77224193f" },
                { "es-AR", "9bc71ad9bbc88fa3dbdb7d0f384b8de1ffa6e8747d1d12857f13a122122009e232c55009f507770573c7ea8d629b648a2ae31666163d63a79a99388ade4ed25d" },
                { "es-CL", "b69ad13571a74f8167ec79f0dc10a1cb372e8e181585223dbd987715541cae4b738613ca9068d3dad589ce8a91fe26d12afe56a8d02438992dfbac53383f554f" },
                { "es-ES", "877839547f75ac3ac2f617e4e03978402e68403f91c535e8aa064ea7c20a17196858c2bf6d22b8f617734062b4c9b1cec61eb01bdaedf4a81c056943b172502a" },
                { "es-MX", "45e7c043d40ee2d0731f23faeb095ac38d250fdd2e75eedaa29e076beb66b184d1aa471ebb081165c5c3baf24e70a2fbb2cb4c7baabb74bc9612360f1cc2b4a6" },
                { "et", "90e87edbe3f0bd6ae07a51c7ff96577a9a22323d4ef9d2a27d9aa81c6ec459098c76419250c52b53e430d69f674551569a09b16b471cf50e2a6720716440c5f4" },
                { "eu", "3f73dd8f60bfa41f85bf1c729b4f648c8a8c46dc68ea5fcad867543155bcfe456c87fc938c3ac924d9a1d9d8e59c6dd23f72cddbafb35d7c308ee60335df2c37" },
                { "fa", "a12b586eb567f66bc450e05af18acca6d5daa65313e434c5287d03f2dc076226bfd16aa105d06cb2bd87c61e88b5268cb0738b9c2fcef8c7270638531336a979" },
                { "ff", "77f5edfdb47127e6e9682f8abb4f0be66270e7ce17fdab86e4cf7f4fed8f841e5f980c0337164be9e0ad56de6e033f0e042067e45bfb216ac1e3b168415f1e0a" },
                { "fi", "95094c7ca7529d172274bbb13668ce392712f58f7148ffff9cbb74f4756dbd91e038ec74bdffc9ddd671e0aa994fe0de8b767803f5fabb84ecf44d744a70de09" },
                { "fr", "83b0e2f472c5233303fd58dadb245079b3548c3378c92e09536f1bc3add83d5d1376a617b3f4aabaf97e0e34bd2047354079d4d5f4d556497e0d67b58945ca9c" },
                { "fur", "b1d2362799606aff14cfcfdd795f03c00f043dac4ee9525ed5023b63d839bb99b6f043e4f56192ae8b48d4607d1d46a9497ebf694a796c318f8ef7f0907a1fca" },
                { "fy-NL", "fab0919cec735864d2d7b04293fb8c87597b29d022e14f5bb2007f1796c26800a8167ec6327555d704a7b993ddaf2e261a1af04ba6716584661316d8d4a39206" },
                { "ga-IE", "7a5f810757b4c1fab48d3d3daa08a8f19bb47b65a8a8398893d88dbf7bb4003957702c18b3a71731414e9ef09c5d646bb85d07c7382e94f687b1f86f74b1d13a" },
                { "gd", "ab7f02f44b1770c25eea94bfc3f1c3efd462405c2778dd30be0029478e21c8c0d30a3ab92b18f4137b1e03eb59f55c5e152688234c2e8e3448436cbc419e4806" },
                { "gl", "4256e5f74839767d34303459d1fd71c4f68664797ec582363c8b88e68ae8ad9d115026227ff0ea22baafcf147db332d5f574a6099fe524f2f55bebb0213331f4" },
                { "gn", "05507767f472e311f67a0c99bae63c55ea698f00e6b47d806af34d226adc9c6a39d8e4d32f232ba207431675aed4713cf1d4d24153da6a14070a0f285d04275e" },
                { "gu-IN", "c6c2b6fd424e6b720e7112ba5844b32769e40de3ba184a64c8eea216f5c4b58081d9aa76236cea3bb859bbdc46ef49a60f17dc3dce58419fff23499455186f25" },
                { "he", "3d7b39805f660e8d4d36fcf43bf1da8af19d10dd9380ad96b212cb6847fc595f6d875152d0c372717cc223e3ce3720bd0a1358b341c94c50e13f96b236074135" },
                { "hi-IN", "0e3ba0fc8e51805e5ea764264e85f0eb0492c715005e5fddf421aace2c180a407ceaa51e6e87651bbd9265f3070cd9275f8c8f4ed3b068bf51662a8d39254e7a" },
                { "hr", "1be7f3045d328bb47eb3ecaf0b27832f0cb528524f60bfcefb37dc541c2c2cf424daf6a1304ed6ee4451b7e8467cfc71300d23c2f8abcb505151e09b647aff91" },
                { "hsb", "6d8102142335c84a2b19f394485ec67a1f6e1868aaaa5b441778275ee3e177cc2852a295e3d18a3ca65e4e2678f7c9a0187963d9469c2c78cc8bc6e4ea07ba83" },
                { "hu", "1f0cf9d8af3a4e4430a95e62044f1d83ad47e6e7e50665ad39619805102563d16123b9c416aa02f021bf1ae79e8e68edb4c006a1f07b1dfc37b95cc4314d23e7" },
                { "hy-AM", "923a677f5e2fa32b6ad4c63125b4795668bdc18a7b32ab6218ab61be05f849fcaa5827291ffd6e1a9db93812af749e6a9a2d7f20cb583f6f86242b90fb442657" },
                { "ia", "95b3a5eb31691de2df0685daac537a417fc8b5afb50cdedec1eefca32abb8933db2efcd7bf67c1f197a93a9b8300acc339f55c7433f407c34a6f2e4671408043" },
                { "id", "7142d2cacb329c4775a7e9658cb73f35a797fff6539ddecd6f3e0ebed487fa32726f57fb3c1aabce5d29ad947cbcbb6a1573e05f3f4bd31196b8c9a443c9d03c" },
                { "is", "46e9d0228b6834dc1dc9cf101ee679bcccdfdb2abec5f80c16c4d5f5c3b88fe3358026560855e643e845df85ed1eb232a54cdc3ca5bd06c0f3bcb46757a65efb" },
                { "it", "ddbd83ccae96d056bc59cd3899d2407ad8bb294ef5a63b2509799a1d926f4474620584a121942d983c643fd8747242f280c807563af389690eb153d4361409eb" },
                { "ja", "448f8875b9abbdf95bac030262b6a0c89baa82b89a72c10ddf6c99fb41e8f7dfb0a84f01efd440b8ef5637acc52b1d69b5c68798a3cc2dea4796e0bcaaed4c2d" },
                { "ka", "b9a09307423cadcefeb3a216934ae8037a4f0947f562b72e31e81b76e9ff33debe0bab4083db1abcd531ea0e2d499abd317334fc1f054c8b8a8f1809aa0b8be2" },
                { "kab", "6fdcd716f295f42c802a8a66d59242b08ff1d7578c24e1fae48585b43427e8de71f03f08a8957269d68aa50a9b2980ef37d7c32fa49852f2e141c54164350037" },
                { "kk", "c0c009ac0b256f1f95f3e9c929df61ac5819f24e61a3f84cb55ea43fa188ceb331f9e95fc96833d049201d9acdc81ea6f26fa97dcf46666b631434edf2d22dfc" },
                { "km", "8f7a08a0fadf00a987334a443f5a51a1abad5a2dbfa1dfa91a76abee4b6f26479143875f30bdc9edd3b9bd906c61b2b0cb558377289f2f8865e572e8b5852f34" },
                { "kn", "6e65aaa27cdbfb23d38231997e1dfb11c6bee8181bcb65adabf48e4ae5c2e650f9bd1cb208c13f00cfdf9f65bdb4668e3589a4a4ee2727db1a3aff1c3d0776c2" },
                { "ko", "5af34c1c21bd3c5d7ca4d50ed076c32fd37265d29c8f19a9bf5a4c0d30d7473790e9aea493b54ff3fdce35ad9bd252e6aacded33b5ea8d6c56e5ba867757f029" },
                { "lij", "8fde8e3662a9641d58e70816b268336f5f7b7df8f03429d40d738680d6452c255a6d42dddb71d66aea5377af74b7b3a56408f800adcfd0e95f77655b2dba0626" },
                { "lt", "c2fdbb7ba26b29aa3fee09342c85ace78769ee8e514a35ea00a9ddffcb36523652874b433549b7ce7f8ec7339f6ecc80eb9739aa3efcfb590b9d0e43775c8773" },
                { "lv", "abb3059a3919953d5edd5ccf385d492eb25a058066bdba109ca90c6d292a75e389393eb69238d5beff86ec5d5f0b85d7ce03549320561414e4ec507a23506601" },
                { "mk", "1cea13246cf5c47c618080714900c7beb9df2914e80d4b0b704a5c2989fe8245a4c8a79840b186a9563df29d1e8f8b8348f5f269d0dd72c01a3aed42946a5948" },
                { "mr", "74163fc99b1de70c81d47af626bc3d3808b78995b91e55615b55522f50b489ad21f6c3411a42d6648b3d2169a999d7281b90b0f5dab587acb4de56b4068e7ca5" },
                { "ms", "1bd84823cd23841fa48d22aa23676c574aebb8c33b5f23455b132edc1944ba5a89a3a188e927bd55e48174fb6b630c3f453dddc6633e2e37ed46f8986c191cc8" },
                { "my", "21de788c1ab45d6546757460b261906a7848a7d993fdcbfc433a1189a1869421cfd936200cb43d0c64c17b19ae75e74203decd9e89bec4dd3b1c0e035abcdb21" },
                { "nb-NO", "3006917931596f5e7fc132ad52b8f07381a6d69ccd18b62ea3437ab950485b9010dd05d79eeb45f621bb822731d73dfb51845699c0931ef6519c6e511e6142a9" },
                { "ne-NP", "4e0f78283f2a03eed9d746f80556c255dc08a2eccdca57b9ef481dd2b9199c5d4eb69fc958bd1159e2906a6891ba33db4cb61aba700dc771a58e91557a559bf3" },
                { "nl", "e8a83dc1ab8d7ad86ac5cedfb8c9ed65752348debe33ff4fb2ffe7511c64d1ac3988b85bffdceac2d4062058b4abd1dc7ea13c62a7330f408da1dc3285addc62" },
                { "nn-NO", "3ae626820415851017ea588594a20ec2f82d50469c282789c6306a3673d89c55c9da41129ff817bfc859e41d1142d424f869e877784f3d1d16600dd595b5562e" },
                { "oc", "ea99f06a213be7c985fdbd6ced8d21360a3169df08820c808624d85c786ac833469c7067fc339cbbdc400bf3b65a6a766b1fb73a83e1e69b48d7a5a4bc7d8c58" },
                { "pa-IN", "0616a724e5a24fd8d0a6111ae55b1daefc0335aac3be1410fe8550ecfe3a3607486907eaa452816346915f7d586bd75b273e533eecf6eceb32512b130fdeb03f" },
                { "pl", "e5782120571ecd99c8ea1192d7f7a8f4021730c6413638df8583c40d62f5b849879b0326a2f1bf9bb51dbb0d209d3eeabe8e80c7fdfede184d0a134081be155c" },
                { "pt-BR", "e6aa9993b3b40a936b956b82410c4c09ad7292f7cad1ed2ad253b601143a086ce3e97d2fe4b478fb89ff931c5c47b4e2f25a52e6075d5497438911afa0ab02da" },
                { "pt-PT", "c1ff66e43059e64730f6bb9afb0cfe887f11c7f8fdc3e44a3b27e08a5a9bfd49d6aeee0932ca242e66e72138305e7e9ad01898067a670c1033837b18b683978f" },
                { "rm", "951be4c96b74fc627c0909233be84df15fe0c1adac31ae91bf79039bae26a09463165de47bd2fecb95cab9e15a36697efa28a7a7404cbc526965b4fb08a2a208" },
                { "ro", "9814163f6cff1b0a180b2e3a1497637dc911eef2cd600fda703368418d27bf5869c6790fa5dadbcb501dbda717fa1f2cffd72e412867562b86d1a6a1e0d3bf2b" },
                { "ru", "c92b9e163a7c3dbc89c5c2ec3109c493d82e43b7cef0064480507bae31127ebef6cb5fa2752ea702cf0dfde5711757499ec8412a6ed1702ba246e1a202acfe61" },
                { "sc", "7ebf47227b3f7fdb1fba41c86a8ee3cdc38a7c64707bfafe6ea452d875ba7425b8e51d8fec4500c5428dd12fa6cbd7b79de04d629907a205dacf9de6bced54b3" },
                { "sco", "a23212722622fcf5618766bd83793747e3b12de9751cff90a4d47bd9e51d3f6a6c96ad2975d392758f40693071da4f4d11cc44d10bb612511621586961d69815" },
                { "si", "40f4226f4867ce7bd0e8c8fb49b8bc902f17284c61defbd70c53bce41d75b7882487c6364cf823f80e89cf97eace8578a1ccef2026603309439e2e9d973b9cc6" },
                { "sk", "71a0066756208afa4805261263e68e5f924eb66034f15e2a11f43a0b0f9004a4c046a1bc0148f28e390abc8d34aba5da65d196662845daf1a7b64da39253065f" },
                { "sl", "7525fc160d70872fdb86ed49ca169741f8335458998cfd86b0a490d10a7fcce072e66704efe804d3c5a2f56c0fbfcf23aa021ffe035f51d7e9a24081e66500b1" },
                { "son", "35b098eea8d013d5a909f23d75f1883ef436aa380192117cd2f46c254b6f961fd1bb40829fdc3645ac3ebc71b9a3366a1aee6594912396bf749299fdd73e7a93" },
                { "sq", "af05d6f6aa433c76f285723bb4e0b819ac7809a19345aa87b727c51036a92ccd48e2ddfc46994e53a0f2b20fea0f7eb8ebe18b0076d0d42afef1ba7ac27892be" },
                { "sr", "1b4505caf64a896203c1f992b0f453d8a8eeeb1edd846483f0f69cb0733d6354dee2edb9e5d64910fe8bad9e89531a79306124c97ba2218e5c2fb23c4081cffb" },
                { "sv-SE", "3b161d0d35cd60260b44c154d61950ce974544cb202e6dc3c57f15fd49673a5e5b44b6eed81f4e0fdfd7b0422db51e18019c334a9f2545b6ae1cacb9dc3f1804" },
                { "szl", "2a726d679b0921aba6b7c887d940178b0a17285b7b308d748fda24355cc245131092a7438bf1fb0ba735aa4a1bb4375ad51aa94baad4fccfe9c0fc0a634aad22" },
                { "ta", "8d363046996c5c465d6e3945fa07aa4ee61cde346ad956a3addd0480f86190539a595ee6aa33750ddff5c8da458db38e78f3231b7e7e16862e76f7de72bb7691" },
                { "te", "9d99d1516c112f081fa9291a97d95afbfc71cd39166525840483cd180f39186dc92c426bd70c2e87698d8097de142adfb39d2950a27052e8e1ab7ad58a979a05" },
                { "tg", "03fbcfa3183da0f2bde5ca3714f92d95cb2a16f34d64c72fa97bd227b0a39aa3a03903245e579f80663d2836c3322f91ad8949aabfeff4b45ea4384c05856f12" },
                { "th", "beedf47adcf0552c8eaadd1858bff0dde0287f56814963844dd982567f74270c88a0e50078d0c769ec95341495aea107442bebb1b24e906636b0db5df9754b5f" },
                { "tl", "fba8d72fe4e6fc9be3192b3ab65f7af1140c36e3d445be0bca5cb54f2162ad5f8264a49313802fb3a89d7cae4512cf7c6fb92daae7aae57e9e6550e7c2db6d06" },
                { "tr", "16f2e3d4a70df2dcdf8612f868899e9ababc4a57c91cec13a5b5ebed169555da56f082d86242effe98fc0de5ac0d305f33df76beb1c4ff931bea5af9db6109a0" },
                { "trs", "309a048f20d69be7e83526f65066d73b3f7ef199630e2d76af1af5eb86ea7fbe2b4040a2eb8fb6ed1881f13ac79595a053afe46be8e64ef973bd78dd6f45973f" },
                { "uk", "0bd031467441b19b39e9aa839721dfeac6d346075b9b86cb5f32355185ce4f2ebdbb4fbb2c292353757cfabac8c8e0131dfc04fa1acb7c36c7d8904a1f2b9382" },
                { "ur", "8a2bf5a8e45f5b3461359e713b987e31ed1bb003913b09e8a227ad68d7bff2cdf0f95d7504f58afffba6a71b6aecb3bf0420241ed2a28acebf3e680a98bb7611" },
                { "uz", "c5715168c634271235af2e4d5c13196c1f66fcbcffbf912f95c2ab660f130b63229458895533c3165b97443f46b02739022572bf25781e0e1d3fe4f79455ea7f" },
                { "vi", "4f04077b6e23871d557d582dc685c71ab5b89b0c2c3d14a4404d31ac305547fc1f05720f7cda417ddcc35ef6770c9507895ed08d60cb1eb42e494fa52d5208ea" },
                { "xh", "1c53cd48417304ecc0a0b5691e330536866751029b998e58aa6b1647cfda18f93ff561c07b100a717c722a8399d5af0dfdf0b6beb3e58b7ec40e877902e3b95d" },
                { "zh-CN", "135d787d0906fc8d1f17a89ea50a66d1d28c92d36ef6140d7c74fb3d81315519dfee7d707af415dd107921cb9e1422e23b2ed01e275bf256d9d9c1c4d5be116e" },
                { "zh-TW", "911a3732e6f5b1b3b4164dd523bbbee622d493218d9b8dc60ecbf66413e3a4168f73890afae8e9dc565c0967b04a2ce1d47a4e3bfaf791de6190fb46751398e0" }
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
        public string determineNewestVersion()
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
