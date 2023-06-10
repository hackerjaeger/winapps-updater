﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
            // https://ftp.mozilla.org/pub/firefox/releases/114.0.1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "25fae4aa76235ddc510138b831131f7736a638b45c2b48b73d7b1722de5ff5fa083057ac0a696457b0cbf715fe15f97b284e182d32a7003722bbeb7b6416576c" },
                { "af", "8d865c894548ad83740f35fe855fbe5c73e8feba712a52949d09d778ac13558f837a17ae9e860bd92919001de4632e157d6eb70d1d6a2dbf7e4852c0b7b5c6d8" },
                { "an", "6902420278a4e870874decf9fae923d5e129b7c0763f76ed6c6830ee21682ead3b7dd906f306ed626c05efbae88a0cde1e7cf9dc254430d5a79f054f4c60536f" },
                { "ar", "1f218b223009054aa733c7683762b5098ab43f971b20e45fe38410bc760d2c990149774b1337830d933c4ea7b1bb4a8650c307a2dd2882a135507645f13be20e" },
                { "ast", "3175928d2c8a5aed3bbff1608ab62db2d2ccca8a8fb5cafadd3912216c7676255231e81638657e9ac589703835f5f1248a2ad41a16c5770939171b0a2ec2b2c3" },
                { "az", "8970ae4b56178d26eb6d66873d95df299e2cb87c492a1b0064c01941b6ed542a6e10af8c347f92a976afd1152d89535587fba9d49b7f4d4466386ca379dbb712" },
                { "be", "9050742bd4c4f724d52266aa66805856d91fdc055120ee4a77fdb9b07df9f4592025eda95f5a9cb3d6a1b25600d7626c919495da13c57bfeb11c3667e036a768" },
                { "bg", "1c3febd2927368de3a3b8adbb8ca68724cc73cea5328944a817089e0847c36434adf89b5af57ae5022593a20d65924f15b59f6f9a6d98fc846254bd342137fe0" },
                { "bn", "a5b86bb4461799650d030ad0d8534a04aa135077b72a54472d29a1b21f335f58cbc0cf5063d307f5062ec833d29d0738596fb7838d958281de68a1313fd21323" },
                { "br", "8818605d9c7b326431f92f33d15d90fa6169074fefdc26d083592a7534dd020f90858fafdc71769c9650a0500a1bec912121b148863e81c3800f31da71ffed5f" },
                { "bs", "672d2314946f4bfbec4d903d5d82c3738c216d2566b7670307ea0ad034cd622c6329351722cb73e53b2aa740506e904bd444771fe030f1172777cf63d162a0e3" },
                { "ca", "7035541a1320ac5146fd12111645cb28166bf0a7471e4b57bc3f71c4f3a45a96973fbf788287da85b50e17fdc6793ad3ae8a8ccd5d924afe670ab3a64a953b52" },
                { "cak", "89be9b138a7ffcaea4145b3bdcf2d911af885c539e2dbbd803b79e0dc5ce893fc658e7ce3fdd446153b5bb14b9d8680345559d61fe5384b62b7ce2de36be8bc1" },
                { "cs", "f69d999e26090985c5c14c0315da73dfa5544bc7f5ae45253d7eff331de3f7783da532ff177c8b32aa9155aa4ddea52cce5761018984139b456de4f11816a530" },
                { "cy", "78bcc47e81a38a8b55ef265b2d6afff12b4cbd86a86cfd3e49f66ca4ac655eda497abe546d1e999850bad6253f1c990aefa9ed7f724fedb8b832a472c9bc3b31" },
                { "da", "a383775efdbdd679e7c3b14284146c3b6662befaf0615add2c57d18fca57102ba954c8882eff76657285aac542bb8ef9d49297ffcbc00d398ab1054e46578948" },
                { "de", "e77c08fd61784851496cf102b7b6a0265d3af0fa090c9ab81b70141b8175a2b4b397c5d0f16d3f1ee4a7bef99e278aea3910eb8f6e3cba4c509004bda8cb1e74" },
                { "dsb", "a417482af2558ab1c8378aae555fac5fea016132d6f2a03ac22d1ad8e2d84ac0121f94c4674cb41fb99442821b7f2325134c379770041c6d9d7305280d446300" },
                { "el", "85b1603f2177610374112a8dcfc5015539d884c9a48b6894cd6bc8a9ad251dc2af53dc7819b3adb505466f3882259bc9e7c0ce37a22114f5a44a006bd278b77e" },
                { "en-CA", "336c75a6f8b77468b235210a4e92833836803b6cd2ecd2c4b5ab026d7711f0fec5a660971302f93708d848a4d11b2a6f67ed7002f0877348b566a205244d6bbe" },
                { "en-GB", "79191afcd9b5081cdf5027f5b12c32149bb2af9bdff810a1fa7cb3aa2f73735b8b0bfa09995e069fc840eb3c3a7483f81ecd830ca5127b0a9f124b9a73b5ea97" },
                { "en-US", "87c6c03f3b8ae62a53d4f45c2fd71e55330c5d361d5481cfa7b80a3dd2039f7cde869f62303058f2df370828f4666f41372b3b6cf62d98410c72fe3d8785efca" },
                { "eo", "6f75a3a4ca328cfc4dfb64bdf18a1d578319fbfe294940913e8698cedf3326b8d5ee714b0f667304fbe085cc0bc4a7f4aa3c405fb560b5d168c036c10ffa6716" },
                { "es-AR", "cf8cb1ad9fd256e41538d819013cdbd2eca0c44e8648c20bf01dea2c6e450ddab213a44a93138426d9662ac1468d14348e375b43a25e790bbae850bd981025ef" },
                { "es-CL", "5ddb9ea855d457fb3acb04bd7673ddf6688922f83d1e09536b826a9aa705037217a3c37763e25daecd4ed970c8bdff950bcbe72632b2b0727863e427d22d57c9" },
                { "es-ES", "b4c32da346d2e7380b086c7f2b2a5f05b3035299ebf3c4c5b73796a803167bec03bcd64ea7b29eb171a98363792facd27ff02ee8b70e9c2cacb79df1682a1efb" },
                { "es-MX", "69187c79044329224c43eef5c79e6e71380028794fdfb44bf189af674b15fd18b128d6468e3d934ad5e31f2658bfb69126e42bd699d416d884fe257701fc0179" },
                { "et", "1a21c975e2422894aa085208f7298c3f2d77c1954cc3717f15a22ad94e60325b279e20e59750c7a42e2ce033adab46183737f3075c01c2475634e97d8bcacf4f" },
                { "eu", "1ec994542d65c21c9cdd3e5cf1a4abd41ae61b2f6f422974bc1e027c6aa747dac80c19bba90fe1ddf9cdaf4ba2551d48062857848b9153e45ec0b49074a34491" },
                { "fa", "0b476f75c5fd430a8b16df2b3fe2cef01276ad0d1d35d1308b0a6db8cf31aa619dc3baf0fb5eed3e835c5d86da7d718dac2c16f2ce99749004ccd1f0fbc4e220" },
                { "ff", "deadfa7fc8a4edeb154e945d84973e05599881851abe8446e7d894d80139428828381f9a408242ba2dad6aeace427f07796a3bdc0288699108a46f71d7e0dd49" },
                { "fi", "4914458a751dac00a0aea9cc3153fa23808735b35f89f5a2a457428c5dc252b51ae181bec633c1617cbddda05f86688c4b4cb0f3523f28e1eac1cb008f5d1abf" },
                { "fr", "f7d89638024a8f8bda6764658324bcb8d1989dff9c953073d4c553e64a4ff4977e3e9f7d7c9d6045b0bea9c6cf3a47547339dc01ef4636eba8fe36c7b63ebe81" },
                { "fur", "fd1d2b9707c7a964c6c21c80ebba53766244cf9e40bfda0b8384e6607dba44c0113706d498c30720dfe85f149c90ee6fb83f9b9c6770faa818b42ef1758a49ef" },
                { "fy-NL", "1f9ce06b5eb83b22c5af50dd242f7ba8e387eb0b82b213795bfd54a71353a0a72e61770594b6e121a2a3a5986f73a972f8319c5dc891cbc0879b32f3e87c98e6" },
                { "ga-IE", "ea311f7bd8f311ab42933fe1a82c86473a562cf68d0d446a482ae7a6ffd28a238641fc27c5a37ceb100fcf596c68f1820cbcc0bd063a725a080657df613719d9" },
                { "gd", "54a2f729f287ee2ad36f0383473f0a596e8a09ced8ab2f6df67282f0f5b4bd51f6a5aaa631c06079094e6a9b09b8c3f24ba7b0ed521c594f279a8a79c57b2ac9" },
                { "gl", "db8190a5f2e5caef0da970ffdf3a38ea50c983b9d9bb109054ca0961e4df70335a723798e7d06355bfa57fc6e65ac727e817147105fce07ff9cf289a353096ee" },
                { "gn", "a31e19589b7fc2ec51728fec89da2d2f37b652f2785386481d31ecc09ada7c9c1fb8a6f910aaebb090dd7b2ac9806d4d55fd29eeb15d8fe53b039b176dc21015" },
                { "gu-IN", "84f44a1c06e60cee60ca8496a003f71e96b7a5add15d005380c3fa4ad71c38ce6fffe2044db20472c89eff39c60ba37400f0c404ed9fc368a7e7c24b9ae16dfa" },
                { "he", "fccd8f15f6ad80aaa4004d23341a827e2489cf952871c93a68a0e9632ceea0e0c47f91966bb0a72ecd121e8d6bc5640f4f91300fde3c58b995d51076e90119d7" },
                { "hi-IN", "acf4b73823cd6f3d84b9a4085fa9a1e96b1d8e89f968448c831ad64421f2b4bede05bbf01124457262e7a9716fac60f9c3ee7b16328c3782a40e4c38ec019339" },
                { "hr", "9fdf8dbb4ff13388266ae1190ee6f0bcb38f843e7a94cac5928115e41e574e9ce85c64f25a6e34f6c571af6074faeeabf307527eec22840e247d22ade95ef4b8" },
                { "hsb", "9445ffff93592066215c407702dda75846f66e2836ebd4c53934ba80e06b12fd6ffd6c4e3fe0a28650965ad8a8eb876c2ad16de8d6f2f74243ebfcda5bb30783" },
                { "hu", "3f34a7d6665ec849892a2f68cc6d562a9d6460a856df449b8b8573253d8dccc6ef7ecac71f1507d999ac23762b23392c46a7305b71e52e721b0d5798fc7ff97c" },
                { "hy-AM", "c73c3d405f000acee7a89b22328ae2f558c3baf73abae8315d4a613f62e27fc4aba7cb3853a77159155b6d71cbc4f7284b0f74608de58329450ee4ddf184cbcb" },
                { "ia", "e77ee8f2b09cb25894011d03b02e03206ad1180a01afeea0475f42a3c278e31affdeefd85058e540f7e7fd87cbb48e71f489ab8efff790d2defb6b9ff44b1302" },
                { "id", "2823726b10f82ec4587740bd5fbc2a40c93934c793817c5d789f88654f09384331d452e894e7506073b9d1646ea9e0fdcb8c701cf7c7170c5cfcf2bb1a5ee7e1" },
                { "is", "05be0ce62b754155e47ef4835f8740210148b0c49c494b8eae869521c19ae765c4ac26d732fb38846da04a822306a5bd24e44bccceaddde00d80ff8f229f786a" },
                { "it", "f45a9c9d01fea99d9904c25c70ab628fc9675d38ba9499adbf815bbc920f95567cf1b63e21c60af021228d14e2273b04708fa95d43fbaa6bfa3711185f2bccc3" },
                { "ja", "a80091b5918360cf3fc559e5629c3f4353047ca4d877650f503032f3a82f246d6dca647c1fa7938c1a27adc807b826153511d89ba6c11be11a7d5a917b14a95e" },
                { "ka", "d1b55b18749d4c8abb9aa5f0f627042a92818b03474514221829b759e399f3032db7b399f7c6cb995d192e7f5833b6e7e953a51c9576c13132737a580821d9f6" },
                { "kab", "3d01474a1b474aafef3f6c731dd3637b10c576add6fced7515a3aacd324fef7c83601deea8e50175aab433b220ad249683002b26037173d841aec7cbb248dcdf" },
                { "kk", "c188400ad6e40372fec74e2d7486205e4ed01e411d6045ad830f55e0a67423b86e2e05b422cf5002f2d2e05a6dc6cd333758c49af2c6d836e66691605b7ecbf5" },
                { "km", "0012c7c0cc04aa18175661a94ae5730179e037d60e128333f089ad5dd06aea6efd287afc4db9c87fa558cb567c6eaef02d1739d2b7323e1c30eb9c5117e392e4" },
                { "kn", "c6265e15e5f2d786baa2b5e072623ffe6f8417f47ae48900ab1f7525c296c85c542937378512eb0c8c4e6ca23ea0c20588fd40276d508480b7f56de22ba197b8" },
                { "ko", "1baca1ed918abd1f47a10dd44b520f820c8f938399ecbb25233ee8b084b8e8901a764adc8f2589860142c2fa8b881d2608d05835bbc240a0f921f21a57725216" },
                { "lij", "035b2b5f54ebb26f21190b082f79ab4884a16ccb96d3bd28a07b303a18ce4744b663db9a6f82507eac10f34c0f1ef8d537304677dd15bd17d213de35c4be1733" },
                { "lt", "4403f6d958c39d84f723e7b4da48dd5c45801c62a510a1ba6183135746c1ba4cc1005feaf0111dc9360529d9ee4f55f63577b44fdf8f3727ead7063925dd570a" },
                { "lv", "4519f7bc8a39c775ca4ba59e033fcd171f59ad3698389469596dd98c510088420d3b9008e21d2b6298fa5ad0390c136d7e0f8d5177e11f843e633ab726273357" },
                { "mk", "15e8d3526b7dbf101690b27b147100a3b71011ae8e2268563885439c0b3f8700ee253497de995c1ac536b8c7053aef1a4ff261d32b28de01357764c9d19e3741" },
                { "mr", "8b32ffcfb9c61a9798a5e67753e5f5d67fcc3e04083c36060d813128864da33afbbaa5a271fbff766dfb2c191b85e7c440ac783384adec429acd64715394f74f" },
                { "ms", "6e8c98a3a44f552b860f06e75e2da8f717b52e254c22ae29786231f6c28ec00b8da54579a8c4d2a65f9b5ea64aff10a34b6175ab4ac34597e4beee806e73f3c8" },
                { "my", "d4b6cbe41727845c6c671bee378da1c85e686bdfeb8741916a83ab9174bc1c678c35a254e864ceec4088f7bdc57954e568311816735bbc54c6630aa45cae1a3b" },
                { "nb-NO", "0a4372fa620335357b3df282015309052fa8fec698133b48cfd0aed8e899356e4b819aa226008426406cdaccdb011851fd90d37b8cb530971e6ee8a0eaf04896" },
                { "ne-NP", "d0e5cfa1b940b4b3b91240f3230365827bacd726766b6779fe32c99ea06c906208ef559029855668cf0c97cbcb9e682f0cf65f87efda5876532105f655af09b3" },
                { "nl", "7ebcfa36a7de780af9f16939f9db63970ed4ecb0497e70c61be8892b1e76d4947873af44bad691277f3095f755f9f4d7f7fe70b22a568a929891a8ece7143dfb" },
                { "nn-NO", "30f0042c8e9221e63a0a79e0b23e0fb52c059462434e5e7cb1430e9951586b9daa98ea28b99c3c2eed0f7f8c8f95a91cf193418a227d26512bb915b1ee31602e" },
                { "oc", "35df077917d4eb4872324542c89e6b5994ba53382b7f6273035c91d2738c5e3cb2488eb113402f1b2e83a274e4685f33ee19cf1615404320fe218916c841a3e1" },
                { "pa-IN", "9aa44393e8f99e1341ad84e75d4c45ed220d44384335fb18e504f34598ca6a535414843e2783832009d0ffed281d976ae11b5af0dfddfebd9139d770374eae4d" },
                { "pl", "16720daaa766bee51303f8e0eeeb2d54f55c746fbbfe0f09a525168592a3e3e34f9700fe225dbe82c7718fd6400ed192f6dc250ec5e6b5f89daf8f57cc49e2f0" },
                { "pt-BR", "c7a7075da11fe302ca2c5beb3bec1e7dd167ac013fb522f0bd8af913ed1406c4190cbe55fdb0af3a2e72e42b7b6e79a4a4c57960fc3fff4105fb8afe281311a6" },
                { "pt-PT", "fe3fd20de21cc43582ff340744f614dc0a061ab12845d014509ec19242553d66ec7cab3ee167b31f7bfc783ab96db85d1118c03ae28f006b291781997cc0b530" },
                { "rm", "3352eabe1fd3c36cf42ad6c84ebaa4945a2a14e6081146490ef16757049a40999b9a79195982b46c5f3d7485b35a8c8c09418435c982907ca50ae89300e77c0e" },
                { "ro", "70e6ffb315e23c42da178e1f126d1fcb79bde9078fb4f7ffd84c38317582ab3707c7d56254f86845defe087d9b40165ef1c5d61ff8e2b2e1ab79797c26237031" },
                { "ru", "1319454ae170c3698a5b7f0cd5a20026a0a06d1685d6ff1779759658c828004a0ca0e56f496fdcef1f537b59794feab6c3b60ae6b4723c8e21c0ed21c481578c" },
                { "sc", "75050bd2e1e5ab1c061c841740564b4dcf8d5f2652bb8d881852bbb12cd72d285cb0ce3f2b005fb088ebde9ed82c0f2d264cffeee16de5e7e61f92e3e9291bfa" },
                { "sco", "e6e9148340533fa22c5e0bf2be8f50b0be84deb8ba14fa3c526649ac99d2366d7f88f5763c1cd70c3961356ca2d0d4b6ed5aaef737c9a8886f9ed213ee06bb02" },
                { "si", "4222308ef375aeff4e50896ceaec0ba90fa3bee2e91d9dcb9aa0695bc18b1ad7ad4de444140353c72ef79b44f27ec0886200b1bb628738de0fd801630ae797de" },
                { "sk", "2cbcf349a29c3d3cbec7d52ffa6f64b1d7efd2f84f3fc0f112f951ecfd0d0d9c490a9c53ea6b158feb178817a6780015828cd3ab067bfd57b0c7f4dbf507dbbd" },
                { "sl", "da0e2f3618cdc394ba172ee9307465b6084bb4114e758fbc6d3647c32513b14da3a33b97bfd86c3622d3f08287d62ce655cf1e0931e7ca926192fe7649b48d71" },
                { "son", "db4e512384fa87730e288e13ee5211e3a18c74ecba2d73e6f49e52d764816c502afe1e950bd973433908aa135a36f31b00955d53718766e831d06f7ac8075dbf" },
                { "sq", "e7568fffc8d94a23b1c2a0f491d5f9a17b58d79e71d58587c6cf870524cc41ef4b3e2309ba21332d38832cb6ed51e2422fa173f4a7b86b1c3587247599dcfe68" },
                { "sr", "12963d584c7b563f3bbc468ea35a054e15d166e4af0301b1cdb6bf8996c7ea1a438c7e51a52cf0b45c62af602d0a962f379824493cea22dd5ede87aab7ad02c5" },
                { "sv-SE", "9780a151e95de5a17aebabedcc899fd33def6b50ce98498a425aba44f3ecc40587661b32f65b6ebc9bab87be0538e50df573297e994f4fcecabd69763e56b6e9" },
                { "szl", "2234c43ee6fdf498de0561c2d528e477991ac2c2df188dc1e8bfd3c6bd1486c9d3e6dfa053aad9937de0a1d29b8b88a93f14543619faf72ccd9e60ef4f2b49b5" },
                { "ta", "925aaccef58bfa29555e2b7c643fd1c99790fd97a59d0ce598cb4bc16e51602a65fc478ffe26958305da67e0aea885d4c90a9be3bf6ef8ec02a47bad2b491c8c" },
                { "te", "5af65f690ad0a94a8fe55f88b931aad6a4402a748ad8a238e85e33739eb6c479b4f3f5b2f1cf8eb8ffc715cf139e7d225cfcb13a617b209d9f732a6bb3a04905" },
                { "tg", "e399ac24dcee85e198bcc0ea1451703f7865485f447086360644b59f969c483c37f519125f86b3c5daeed9ebaca2fafbba9a5e587f0fcfa4980e188d700eeeb1" },
                { "th", "236c21f6772ba44bd61d5346a9876f1a3f55210726998f720e456f2b017210a38db79f3788d3592f249f94686796883373885e8980648ced9af3c8086ccd8ab8" },
                { "tl", "6242c323f3d72eea444143bf4d028e7bc4903980f818dcd4d94c8cac0bbaebeb3ebe2b7d6b05e46abfe8507da2d4e8e7ad408a324fa2f65bac45e2090963588e" },
                { "tr", "8269c0b2dadcfadef25ddb156e24386b2e8502df947685dfabe523cad52ad7b393ace2f7d7c81e1295e8989711b458dd778c2a6e798108dfa4480de9eef06dff" },
                { "trs", "bdbb6d8abf99d02976f94a0cad4d8241477ee08598e718c3959e8a23eed2d0b8a44e266980dbba766fd0dacfd22370833e6e9316a7c58f23de7ac45112a03118" },
                { "uk", "36d33c8a84056f6e0387c1f4b1778c5453944ead9489d8fe5481c832dc5883b66b5fb0c91236b8c53e20c6ec103fcd76360e1c2c1e49f04d2a8967c9b616cced" },
                { "ur", "f37aac7a065ace80be72c10bfbed1f6dcad738da3ce32912810a2e11668d478a2aefc2c3cb4df392cc126056c20fd20e9790274d4718a6a42debf96181241d24" },
                { "uz", "c384be961e20feab9bcc20961f2ed01562e5dd0530a7a21db113ab287ba1edc7f538c984eb8a48dc08394c686a366912f385fa67ac56b11d4d43170329880544" },
                { "vi", "7e4bf445f6ca3bc1f8114aceb8409206d08783467fa590130d3f7cdf27ca0060157142abaf5e9a9339ee9c40c46544b3a4d354f0605e44f4d25e714ab0d2c05d" },
                { "xh", "a2e209c30a1bc1da4c0bd755ad1e9b3b176eccdf628bcc2df00394e1209a8a282834373d4547a4b54ebe188b6f9932402949456a9381b80f101ddcac7a449d11" },
                { "zh-CN", "28f3ec5ebfd521198f96f1fef293e71c4ea89193026ef306f00e82b2c2e84ae215322782609a63596b6fa33ec514b87ade54b849a766edffee28425ce3390b22" },
                { "zh-TW", "162143f29fe4a78c1a04919f5dff3345487719ac092e679c5fcca263d4adfe5d9f3d6c08ff0c9421ec7cc71b323984b01eaed4557bfd0716f17edcbea452c2cb" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/114.0.1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "a01a625a99ab4f3234d5890277147de98617528f1bac99ea2eb26cc94c4e5e871fe9254bd30796af851a896fd7e391b3c4afc8ff00af62b61f97503278ce3bfd" },
                { "af", "17f95870df995c30c47a7ee22428d5df85ad7574b9d12877e237bd0bdff3fbc2e6b48239a9150f291da39c1cbb9e0d0b6062de7ecea240f228b6d23377642e58" },
                { "an", "ee97c7007ca1b10eb56c3bfec709565dc93fe8c0e55611904f1c675d5d2aedcb2dd9259307cf6ed42eeb57c5041711d41664fa5f6fc6b5158686e35e644c8540" },
                { "ar", "424ee6fe4944eb5870a903a58e4f4edee74a9ffaf3146f129a0e51c583b033cd318da98ab119364180035a15612a37a6bc46807633ab2ed842022975891d7030" },
                { "ast", "3b50dbeb924d7122019aa5e3fffd31f2fdc73a0fc425515d803723e314a4bbdad5de4e38ca5e624cf61b05cbd36268ca4143ef2016e23dca3e714af89fe9bfad" },
                { "az", "c70fc2b3aacf3e75666e5abc2c543ff6746feb355e46a55cbfa79132ac6b8971182cac2916fbda79a2b9063ee97a4ca5c630489dcf6befd966dc818641d6b767" },
                { "be", "39bd06d3c1973be25a454924540e0a267d9130b844385aa6ad61921065f5aa7848290997b49934c35ab92b308b07acbfbe2acb14bb341f391b8ae34fa5c65c21" },
                { "bg", "af48eee6dde17ed302b1f76e995d437e967fd2e70162ee885bb14a525cdf0485a7c7c218dbe4d912a09ebea0d1f3c8951237f16522bfd548d1b2759813b805cd" },
                { "bn", "c34782e47461fe831e60de87d4ad3891b96446303bcf92ac92d986bb96323d04aeadadcf7cac3f0a147ba17ef01ab861d788c17da6f427da8fce168300ad74dc" },
                { "br", "e65d28f39e237ae3cc2e07bab0616fe1bc639da3a8bb44160fda33dd9e5b1fc15fc342ca3846318863f7657927251653ac77849acb5ce952b271364796be5e78" },
                { "bs", "f470f0cdd8284d3101c488c349829ce7851b97021509e3e90efb5e51cd2d625b8c54170b823b3ebc5473addb7411a39d0af1fd214d7486f1eb6cab775da4f762" },
                { "ca", "25019bfbcfacba590838ddce01b2c2a03251523345cf59f8d9876f982632315538bca3403c95732cfb452f2090529f759df0e8c32a2330607cf29b55407208b2" },
                { "cak", "eb0bd1ef0b7900731af34a470f125f55a2a34f48e893f6e88fad9d288a2451e6d79b2b6a6d02cca71f0b8c21ef8f0095e730b731ed922991124df169ebea3695" },
                { "cs", "2c27115cb45230d128afcf819bf27a51e6f6f362e0bd3b06c2895efd9993dcd324e975410545749c6a7f2b0d20ef320862fc78a46e1094ecfb27c998a8398bf0" },
                { "cy", "e55ee9bbaa27a1be131dd7591542607347348802a2179b4ecba7e54221b386ed60a4361b25ea9b10fab3a2a15d31d4f315ee985cc470bfea945006f69ec79c5b" },
                { "da", "ff9a5dbf842314f9030813df9789fe81fbc0a74da42291b8bd2e19af4275b71958deb0463f10bdc70b42892354d23b51bdca064c806273e6f3b64eca62da4ca5" },
                { "de", "6f2cab3149afe4842e252c41f3fe3f16fa12d8d0f0d005c60578f929b8808997ddbd8dacbaa6ea7f8573c25a8d0a0c8434172c1876c81271377081ea75b01249" },
                { "dsb", "6287de36e2bcbfe9fe0348fa206e9e067df5c94107ad0b1cc3d57401c7a3a79701fc423e5c26e41f81e5ed915980af17a42f41111179e43b25c604165440692e" },
                { "el", "b89356f8e304d1b51a98f4689e342cf3b377d6a0367c5839eabce9474f8800b2ce77be2b1dcb8937202e80e9a1466e100023fae2731400b8ec9b4f06ce08202a" },
                { "en-CA", "37e0abf1548cb9014548ac075d59d3d58e99932d66bc98a6ad319ddbd2ace515006e2ede59e93afc901893a8b61c2397e88bdb88b47063b573f038e52c5c30aa" },
                { "en-GB", "fade093f809badce3050686324c8d125c2b560f4d752b7093463adc60824c5213f66076125c50eb27acc921a776eae491118e6601d56cc613dd60e0a259fa704" },
                { "en-US", "3ca86c9f04f2c072f68ccd344df4d2e5a68fae07f0dfd91112927c2e8bd0f0761f2884b5c661c59e5e36a16389b653f2ee01268cb86e1bdaba4f918fe64abaf2" },
                { "eo", "8ac61dc9e5816234f23a7356d1d915346b72e7dd2debb8a09b45f862c9c2dadaa81c855e2191a5a480c4bc0c886604149dcb221c329ee6a664eeb1f05612db0c" },
                { "es-AR", "7f80278a8f7977b51d00c136bec3c30e20edb3f54153574effebc24bd0fce6ae74d23e7ae988b090343f0985dd40b9d39f58fb7df330130bacb14bbbc5a0106d" },
                { "es-CL", "9cacfbdd97e9f9e0357eef52156afe4375dbd303d3761d67307deea67b46ebbb331a2147613eed4caff2a3589dfe98c7c1d8e32453ab52075c97df088f05d6b8" },
                { "es-ES", "83becdb9777660628a8d996f9f955260dbab4f7fc702184c553c33d544d20ff49ec26ddb7e53864d940349c1759b9e0339f3d89c0500f67605aee405afa53f8b" },
                { "es-MX", "3aa5500eadf7a46143850bf5eb3a210e07e13db66c8535528804746089efc8c00e97706b7859bb61d1ff64a98782b72e0d62d6160e0f346be254875a8e5764d5" },
                { "et", "500d8e2a01d894f473234eccd4169cf6d0f97ed08927b2c40fe26bcbfd3d4805fb07416613e2099a010e1fe131fd42ec21755c4dec91643304f135ebdc4513cb" },
                { "eu", "69fd0facc750fd65da15917619a23b5c4ad498295b4f92fc1e713375357b382b842e0f767ed7971f84482fdef76c53357202aeab0f1245e56217168b865607eb" },
                { "fa", "d6250a2e14bb9e982c50b97b73d9da2bd1ad4ffa15b79f652f2181795222e4fe3de3df02381af6910f71fc2fd3095fae4babbc472c203019efb6a34883e896d4" },
                { "ff", "97e0c44f65b50a1da70ee8a9529b74b75330687c9e74735ab3a90eb979c1d66790115a013c71d40e2dccdb8dbe500017aa6178c76ec70b5b095c8f0275780276" },
                { "fi", "dac26b9a33b61b6add8789852317867484ca4d856dd81aa3c40907bf0599aafd6c0710329015bdaa4fa1270744d4f37e4d911c3a8cb7f7af26971a1a8ace6204" },
                { "fr", "188867719d78bee6e58bff7f63f9bc170f57e63a06933c700bc0715dbe60ba5e1d1b9f7f73ea83273babfd44bbda93cc45ad703ae29b5e8ae72dd7a946b9298d" },
                { "fur", "f18ac6ba083d7c3ee3fa9b7fcecdd3ed019fd4eeb7368c94245fbb3c02b11210afef144ad85e397781ae45be29fd1e9ea73b76f8f944797420e5e7dcd5fa39f6" },
                { "fy-NL", "3c9e8ea3985903faf0af6ee72c7d0d6210972202e307e89a31f879f745139244bc6e5cacac424aff2df16aa16e4da2b50493c62d10c7a40c8227b40022e5dd64" },
                { "ga-IE", "b1474acf317d827d07960b3eb2334af7c92aff77672ab07dd7d1399c64716bdc9ecca559d5f163995b838a1330c3d1d6de37f605cfa973858528a717b9a2fba3" },
                { "gd", "57d9588106c8b53551a22f7c5ea39b2eb091b9d835f9b1a5fe38154d9fe96385a7a6a21091d5c8ca746acdcb506fa23747a7a4d11c3fad1eb39217112c9ce11a" },
                { "gl", "eb366b00652bebbfd2462676054a41671e41acea0813ca60a192b393d8d6b0ec67cbd1e629cbd51267f31b31231f6f088a8ca9859e62f1af575b7646aa22f2c5" },
                { "gn", "973b02c8a247d284e6532adb176c9412c538c4f630c8a165c0660d385c951c84512de69a95be6f29f6a5fbb4fdcdf25977fd17cf4b60b7d10ebbea9f766447e9" },
                { "gu-IN", "fa96ec61dd18c3b2ded98da3c99264e5423ae32fd50d212422fccafba1e169f7048aef71fac45788ee639a5731dced9a946529f87a03d64d314ae514315b64c2" },
                { "he", "5df76f72dc4fd69c7f765882c2f4408377cf9ad976f61da6532b3e001f0e6e870b0aed921c57b1fe64bc4fdb185daf5303998b417b84f150238507198a6a6210" },
                { "hi-IN", "f71118c0257f310aef6975e882cd4f6c89196a6dc229fc94abf9e4580dd33b7c1ffcade51eeb25501011bb6ac1cc0ad160f36684f55b49487879ebb7fae98b40" },
                { "hr", "f619bbc3116ca67e241aba4ebb20562579b12f5014052a70eed06c6d3b3ee6d89212db2253ba3e293214eefd5c936c08863da9ebb15233f95d7065878ec67a90" },
                { "hsb", "25607beaeb81df55552fce93e681e82ce1add1a74ee70d312be1ce3c1ec9992fc14df80018288821b9d872034ccb731e39bec0ec585952fcf26232af42b9ca7c" },
                { "hu", "38a8ba0b9a33dadded388e8a756907a5bc89b3fdcdcf18d32b5cb42ed839d371c61f461dac06994be894a8988e2929702a44bb786a0495d9f5e948690c693685" },
                { "hy-AM", "6e4db18f4c37145cbcff87a0907bb5dc82971440c21fa0c85b7d2c9fd6cca0ce2fbfba5747f5bc8142ac451bd047384b1c03461cf93eacfebfd053e2602690ff" },
                { "ia", "55de7a86f01068401577aecee15ac7290bdb404f25312ac6ae1c5937fec177a6fac29a76a8aceeafd00f0abfb9a8f7a553194d0677092f551366db18382fba02" },
                { "id", "d47f58ec3fd2ab161cf494717139acf90cebe784299f27debefbe5f7f0fc08101f894a8d446e51057fa0f41ec6fea821f5a6a8c3272a944b6e25db79d7ed3da8" },
                { "is", "2f271aec81c032761ad624e1ecf67deef9ede9cacf7b7734a647e158f5d58e33511cd0f3cb8b4ae6951ba297740af6d3d2b387199d6f4dcd365df7ff53006c83" },
                { "it", "153f3d93f69a6fe593cf6b6e2efdcf988f67abcc5e60b6f9ae7694daf252dcd1e8dde814504e4cfbef3a3e36a195977506909646d3f8b6798d29479e4b83d99a" },
                { "ja", "fe5adf4dfc613962e90ecea29303c3768ac3330561cda539883deaa2046a02515670e141911460d6a3109a2432b4ea8f6465fc555f684c474024d7e183e91b3d" },
                { "ka", "f078f23064f28c93078d5da7aa637196e4306177020041137dc58660f0fdca1b8eb1cd861963063ff582df03ac355aeabdf72bc7aaab87e301847b7b7b5570a8" },
                { "kab", "ef6e66d5263c69a00a1d44570b43b35116e70aa0ec65430a3f046ee8125290e333fe47a2fb41ac70ed421295f3a34c8a654d12071f75ef93d4afc63559e858af" },
                { "kk", "fc84acd2e5f6e17c8bd50cc2ce27e451d64784d0e6d9ed318db33f59c92633f7a59858272341b2338582901458b75c53bd3e8314abb5d2db8e3cdeb0635a67fb" },
                { "km", "281108d39e7adebd3e6426ffba63ba25d7f2846c706ba6ca445287d0f9b0e88c195184a212ab35dfa727d10c67d128ab5922a7b4aa1ba3bdfb37acd318d11347" },
                { "kn", "b2b134df3747d8865b2bd1040720259d2324c265addf712470f19dbfa90d39f4e11b2c93d811f1cb7deb96ffd3d9d83cb07c90065f72c7fdb10cdfea05c35f5d" },
                { "ko", "c6748f1764152a0ae217631eb7534f829566f412b9d645977c23019fc543935bc13a5ab351546e9457c3b2dfeb0d9214a0993d66aa221282fadc7cafa942d16d" },
                { "lij", "1f87c4eab4954bef320d6aac5c7026d87865293db853f33d8c49aec60cb72235de1ff877fb0520fd76b19d914cb8261864256a34b3b3643114a53889c842ce0d" },
                { "lt", "b1fd3ef383242d17a885cf6e3c95a3c380717e267a96fa5b53172a00c9e050a30c2c8f8766b0d3d7a568d171b2e74f14cc68b118477847fc222dbe8e1ab9f88c" },
                { "lv", "7f2b1050a84e43f5e739a3f049a5abbaea3bf05285b092b6f471d0f0f6c53d97ca408b9b0af49c8a27b911aadf91b6d99d4ba959333e6b5af311d9416e08434d" },
                { "mk", "eef2ca0dd9eb68c2aa7e95b5cd0afd31ad917f407cc3f597941024fa548ec817e05659f9d001f447d3b842d7cd460b9eb069e15aa1da4450a5006b82b423d323" },
                { "mr", "4a02f639012e368fac72141af19b2e59bf43a70e2fa4d38d644dd05ddfa84b71dc15a8f036d37f1b881aa643c5b890c31a4cbf947f0ebd1b69789a4dfd96888a" },
                { "ms", "3c9db2ebebdc203e72df01a5a08f2176fc3ab6649639f2c12e20417d107b65df01cd92084692483a89488dbec68b7a821e0b31f6c678aea49e4966b51f52e1d4" },
                { "my", "35e2532e1fe7de118d62ec28cc631f0adbcee0a6ca8fbfdf7ca69903c5bc48155a8bdcf140e48b91c04e4e51984ffcbe3cea766b2a27d376cf0538ee3dd23317" },
                { "nb-NO", "812de6453c7dc46256be73adc2c85ba1db9bee1b258dc791da8d392a98312655b54ea099a37528f011fd586fa358a36d939da3ae4fb3110efe985a3eafe8b050" },
                { "ne-NP", "da1130f6451392273e4b4fd2103423277e37fb9a1e99e0c5d54365b492b22c00ac87a4dca52a38f862a3f34e977e02ac8da6f9e76d0717eb84a9d7fef25681a0" },
                { "nl", "e726f7627fadce5c5013ccd064f24b0b0473d34d7fd7f83dfbb891f1c40dd16c5c331290dcdf30736d216baec9a7cfb4493f3d86139ba5a927ef16147434e8b2" },
                { "nn-NO", "00b168a175010ea0687ea6d77874e1348701badb42baa92caa71aa7ffeb1f768fac20d66eea655b4f23cc1d29d6491a57333075451e2d4b4e22b1f671fa5543e" },
                { "oc", "4441ddca525be5dbbcab8b2b5222c8d560d479d899ddbc7a2845b44fbf9c83958a159360655be63933932f7b3fd635452d69c3ca0a4b6fff08df305809a4a592" },
                { "pa-IN", "c6abc2c8d5e29418ce76f6508e7804e49f1775bfa2718472a46ddf04f2d2d78278771e3b1165084924e4cdc128293492eb7f76249aaa70050bfc0172c4888aa4" },
                { "pl", "fe767cb6c1c0e66064ee245f68b15de6943ff3f30d4d1326128f31561e33f3a5798b1fef7a72d476f0d659295a1e3fbb3f7599acfdcb808da84c30e17a174214" },
                { "pt-BR", "bc2f8a18088f8f8db3358c55860f900dc51fdd3e78c4e79af12266fcc35b35ca8ac90bc898e8a0b9b84042afdc4844ab50da2283b8a2d4861f16c6ae9679087e" },
                { "pt-PT", "7ed8838e02cac9c422222d5b6d43b2cd64a883358bd4a1e0fac03547cb45410bfdedf4b3c9c5612135f3d4af6cb4676cb82ea3c74b9a0f9dfbc78334b9710488" },
                { "rm", "1b6dc1e93bafa365841470f956e7308e8c4da10adee55c0abc5f276fea50182e43e032694ce731bf3c59c4c39cd463dc2e614235dabb4f671bcbfcdbdd07457a" },
                { "ro", "3a9f76944f62c53de74a7a8d198683449b17841e18afb8c04ae72d8bcd1f07120041cf99398d6f366ee250ac5b198a75fe64d1c1609796182f1cc1e4d04e1306" },
                { "ru", "2ba04fdac2f725a52b473208c36c9010e937119106a52a7eced1e53b9829b590fb95dcda4e8fc95a84770851a4a40d965074cc8091b6ac140045700a97f3b286" },
                { "sc", "8ad0958453b1e05f9ba4b14ff8cf08be7e48c719404184c206c77b86195f5c5ce9f121b0f9f7ff196a265bd9a8b2a46fc2a907c86a8f25037d97774358219c50" },
                { "sco", "0db82d408109b93a9f18234f7b84e07653b39e43ffdb2bce7901a231ff92ef912ee0302aabded8bcaf8617c664a652fa240fe3f8e151922ce39c0755254f9315" },
                { "si", "a21e7f3b1aa8e79043c9976405009ab1f34746a5f1b1fa18069aacf8cb8dd27d4104b4a3557686a6ac6f4a37dd08a048a89748b2c939dabdab0aae777ba1e677" },
                { "sk", "4188aa7ed2097eb038f7176501486dc08e95e4a648908b737b148192e1d16fdc00f7529c25d9bdfc02445daa2a892d4f9b6ec792bef3b09c3ef14693dbfe70de" },
                { "sl", "2c383a4b3204a1d387e7859081097a259a3c4363409639e20be0d142a037501c6487a4580e459fb87262dfab36f511113679c124ceb27545d50aa467e8d7089d" },
                { "son", "d38c5e0acdedf1123adb0c0a4544710fd462bf8c894e83702a1a12d230701182363e91fc295070077c326df4b3cf4a14288a1d6bd51d09aeac1004426bd7d8e2" },
                { "sq", "2c95c98a95ebfaaf4d41cd335b72b352d9c344d85fbbf5ba2633f81721c2102fde1a176215fd7a4b7d502818a7ed6222bd2c3b21049f7443031e56cbe20397f4" },
                { "sr", "df1fcbd69399061556aa1636e9d4d7da3d5cb8f4b592ff768cd86aa9392f1a88e9c4d2a0016d902d757795ed8e3aed28257dd72dc64ffea4fd3ec4f2a2fbc4c8" },
                { "sv-SE", "f85378fed6fcdf21ff3dec00d18df1587e806c9e96e7d90d82edcaf77100114980fccaf11d684db7e9cd3f31f1b3e0898b4b563a8def0dfda45db1c065b3ea51" },
                { "szl", "e6587af379671f6b963e28987c4fdc8026a339059a9b197289f223f8e3f7a40a771e7dd8ba182caa9548601353e27d1bf96ee5b1dc1f3319a834f2a5ca6157ac" },
                { "ta", "83554f95adf73d72f66a9d9e02902466968604f5e87773db96bf0101e97579a3fbfb9c4cb5182ba82c97a6bb9363a61969b38312fc0aa205bd31589e03c64685" },
                { "te", "029f7eb422808dd646d75fe9e0034c4c31774146129f13132b1e263e5bbb6c4d81857175dd9145f6dd855a819a412ec232954e3ae450fb4e97dfdeaa3ea2d9ed" },
                { "tg", "fa2984110590db4a625a9651259629cb192c095ce80f3ad3c405014f429f30d0fbe45b15cfe8c6c4c4831f7efdb214a11decc406fe604b88bc6005f1b665b3aa" },
                { "th", "f75373c61a976857325e539d0babff2c7fc8097b154579e57c5b6e6c2312bae2680c9dd193dba52ed87125964fb6c9de037224a4a006cdd53ed64f0985730da8" },
                { "tl", "6fcb00c0179b0edf6c13d47894a455dae403b2428f2a69e2f2c7010f1bbfe7d6b359e44f655b939eb31a66de40628e9faa6167e14b7ff7b2aebdde6d42a91a18" },
                { "tr", "5279ea70eb3e1e8e0d248653e98ccb2c244e5927e07a8d6ad19dbd6c2aaccb333b0cda374626f172b65dcb0a0401ecc971b97119a9a5123b4cc919a322beec12" },
                { "trs", "bc3141dadd44aa278e5b535654de7f736c469121c796923f1060b882600231e2697428db6e3e4a29c4af600b3f6bf74cfb9c23de1551016ea1bc555813494e93" },
                { "uk", "1faab5d5ee3ad6f52d32c3bfc63e90915a0cab779a7f47227ce3de5d594c6dd11a7accd11aa01683d9b9fe6ee36328e4a7115068e097dd32f91e864d91790b36" },
                { "ur", "a246f887395639d3a8eca0384c10dce4847b9e2d62555b7232fa05c47daadd0d6de7ef9c4c363a425080a8476c82c7cf3bc000fe6fdfd79c76351d8222b587e1" },
                { "uz", "c40b760965b4db3f134362cb64c0b1282e71a4d971e359c3b647a6a2c5387873583889e63135a997a65b69bc983b1eae68433df2c9b4e6a3f33dbf97d5e9f41b" },
                { "vi", "1d459d174270bc02ebacdcaefc19bc451be2303d6175da1ab526f78a4e33d7c979010581199af5505957157b5c5b85b335140cf3d810e853e719c7752ab2eb8d" },
                { "xh", "c56ed64754e1e968fd70202091599d1adf0c746e77529074cd0c2f82704afa13458a1ee1d12e6dd400126da1abf349cae81c80031226d60af65c33aa9b8b8754" },
                { "zh-CN", "57d9dff30b2692317460adbb080603b2230b05d6f599c8c6840f041bb74f4e9a5cca77e103ce282eb91a36b70fa42bffa8164e276ef666e10e6b65e5677a20d0" },
                { "zh-TW", "22925c7821e8818b33ffc6d28e82808f79b5c698a8b0f2a5db9aa7c390df19599d66a24d0d2a49da6e51b064a3811eab86abd2ac0d47540f2dd83850bbb93f48" }
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
            const string knownVersion = "114.0.1";
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
            logger.Info("Searcing for newer version of Firefox...");
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
