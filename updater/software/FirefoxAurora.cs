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
        private const string currentVersion = "120.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "a353a165dc5decbb23c10f8a2d560103c1aa442da89d50ebc3d17ad2552a3df2cbe0e19d0412ee4581de6a55093b2798acf0749141b3772174655e0a49cedd55" },
                { "af", "c7356e4628bd970b560cbf681e555e5e5dd62f682e2facbb2bf5cd5e4d5a95c150cd4b475b39ebcd60bced121dceff8c8a074114091f0a02890110461d768f51" },
                { "an", "671b78e57f2b0193ec634f45f280a38cc2eef3bec9ed80c79ed26e11bf7fc4b9b67ec4aa2fd71f282fddcf122de490fc824a2373a1fd215482d1910a0b41a90d" },
                { "ar", "e8d4021d58a60e4b0fa126276bb5706c97c9cf7231c92ec4fa4a900d0e3216e89fa72344f8729ce48555cb1a470ea3f6d684a1e5ea397088fa84e788e454e62d" },
                { "ast", "54f16ecb7bdc4032ce0b4fd652558f2a722ae467fd5b002da4fc8c2fcaf3a510f529dee676705ea8eb2bb82fe8597cbc5306ed43d11801fa4bb021d33314c527" },
                { "az", "61017804e2c56c34a4aefbf61d14365b090d5053be4e7284b463bfb1e5b0fbf505476c84f1c0eb026d8ffb60bff2967a4d62f48e53998e7398f39c099d0c65db" },
                { "be", "8ef73a7497257f7ef63ee30ecd422bbbf90d3f1376665936492db20df92dccbde6877593ecabc78225c83b033297a671f628f2a608dfea806aef4886d8c03855" },
                { "bg", "7997234250c054666e66095876775dc8437368bf630e4ca6763a7ddf530259b6565fcfb9bf0cb925ae7b60a0dd8b81dc7a9d712da76a39b386f80e8490a67b4f" },
                { "bn", "d3798e096643c16b7e70aac1ef8790d681b0824084020930833fab3c9ee3140ce76bad249d871f173a2f9aa16649b22f474f721bd7252c17031155dd9b509c65" },
                { "br", "27e152f35f14de5ac0deb206c53a4a4ae4375e5f89a52d5405cb5fee74cc31e1ce93ccdfeda97829b6b53af59f80bf996dd7730f284d18e1b350d079ada6e07d" },
                { "bs", "b3bd3447c846bbc9c05ff4dd4d2e57206233a90e47f5f7ea2a702ad3bbbd11a837130c70053255b4b5a57cacc3111733ca9f561b40f4a4772c21b00bc360ac04" },
                { "ca", "6cc1cdd3a615eeecc05e019936794f990963c2c773384aad91cc3bdfe24ce238147d3c74b39bf7581a1c325c4d4dec8896c7591985fde6cd52ae10817aae71c9" },
                { "cak", "a8e320770cb2ea0b8b24e584e18dc234b387e1f997ad01f7240a32c728c1327df6781fbb16dacfc33bfdccadf75dae1902adff11973ca71f21d5e6368177f120" },
                { "cs", "08b08c7e7d6f9ffedb8cd4264a84d78a00f59322b5832536f0faeb3110a49ad09e5983db0ea6a77975dbf3b4b696ef633d669e8d5c25bebf4565dbbc82f2b9a1" },
                { "cy", "bd7f2b8ffb10bed0fc24dafec76708ad9de8f1d0aa58979ada1c7be736b2d00b9afb3e5feb881b0bba3d653f75903ead3a265cc0ae3666c30a4fbf14ce5ecc63" },
                { "da", "68b332b7de4de6fbc9fa3f62373ae3ff26310635c004022c60306644fbf8acc18a90f22f024e5388bd5812c13479d29097441c83b7d4e171f3ddd3d9cf659c9b" },
                { "de", "94579c927e2511dfcf6849dc95080c9ca0e41bc376b15bc55c6cf9c06c24479c7acc23a06382c9571c590e4eefbaaf45756ab978630d32cad3bbb5be577965cc" },
                { "dsb", "efabac423a8a3e5a0f39e48035e6cf22894504efe9ed49289cf903a8f6ae88ad007077f9b8e5f9388b94948cc874ba83f426dc287d0d74e4e81dd94ea032b33c" },
                { "el", "8292384adaeae71b2c292abec1ef0f119e6bc76529360d93d1eebc0f74e1e82ecc6fe00715a8be9ed179db14ddd64a942ec5d04bdbbaa5c056ae6709a7e81616" },
                { "en-CA", "9c66ccc09b7e9173fcecc8f66e7663932084a48e54e5b46211d0fc08155215c353b06c9b6d0eaf87b26d67099ad435fb2d02da953761cd99ae8c54b18fe09fc6" },
                { "en-GB", "d46c6be7d33d84978130a5062ec65cede99acd40f1115997fc16b94fdbe9edfabf59a98f37ef1ef7a3978220e9532d07a399f386a3c554a4e2d58095f45979af" },
                { "en-US", "af6298e43938ae76e9ed833ef5c465bb5d7f1fb0eab75cf77aedd9b6e67ff9604249ab22d3a1edb9e9eaa7994f5d9926a3f40a2c86ebf6c7c8a5dc80e1fa9505" },
                { "eo", "ae014756fc7bfd9cfd07e4fe55a64e361ac9e0fd79aaa5bac1c883947d3fdec71fa9538117228bc1bbaaccad500a7336a8d12fa043dcf9ff136bca9abe490681" },
                { "es-AR", "3ef6a8f9b6d3076fe5dfd09f365e060b5b9d2c4ca54c3ce6005db55e91f3c352b66858d8986ba012bc8c73dec348d6386b36ebad772e14d9ee1630d7bf15bd2b" },
                { "es-CL", "61537d019d4907c7296da149e1074b552bd8a66322a6d24244dcc7f35bc1513299ece33c6649fab64893f6e40bee60e9780a586f0c45f04294db3e04b7d7cb72" },
                { "es-ES", "5d577f1ace9215b2083ebb17106ef4b886183c88e6d18b939e3fc3fda84b120aaefa6959aa5ee79897e4c09910eac8fdd07f35107455cdc80dedc537505eb816" },
                { "es-MX", "6e4691fee8a8edb2d678ec29fa630358c30c08095fa60655b70b912c6c5c771d97b232d34710a851e6be28806ee8aaa22c83afb78bc74efc15871222a2a9f527" },
                { "et", "431474ef37bfce9012b18258626e84c34eb8750ddb7e15cbe2af2c60ffc224caa6543e1fb695754ddfea6073068e21aeb6692bc8f57222941ec0574d8f01e07b" },
                { "eu", "b8fbcc62d755da9857f6d7ed5669fb024bcaeb9b2fe610332b25567469eaa48d6810c0c1dbe8ac41c36d4b619ee71e5585ef1c41391e00f06ca67f5cf6b34104" },
                { "fa", "98ace3659dd591dbb1d25331b9fcdd096f2299e23ae56f8e8343f425b863c2c5c04bb3da410d9de4911890b238f4f761fca78faaf6e6b5ec416697a03436445b" },
                { "ff", "71759957ae027dcea7311f37cbe4340f024a43f0bb302eab05eac7545c8ee3c803e961473c9c014f22245f12b803d746c25de9129c17583a8414931ed37bc8c0" },
                { "fi", "01490e37b50dbeaa95414b3b8b5935d6132d51b9cbe08ce572162b8cc0f6cc5a7154c4254b0f979fef8b7bc0803e023241d8c0b61bcfc93b94dc6c6f83931c01" },
                { "fr", "9bfbc64caceed596bea1f344c22e4f0516ba47cc804601c922973f37907f921eec9df1adcc2a9337fb6f5a5ea681771a98b22501051c2c069935240295080dcb" },
                { "fur", "fdaac4b8e7291ddde9de56be108d22ea4cddd6beda2d458c9fa73cef7c717941a32f4457e6c06209b685c82942cff9b70ed9d37ec653b6ca643ceb02dbbb4d98" },
                { "fy-NL", "32402c97065c55736d73e6074f107b6a8f4439de50b3d29777df8f2be12c1898486b2d7e552d75e92b6511942ad513b955b539a65605c86bee76dfc718277b21" },
                { "ga-IE", "8498712de037c355cfca1f42e94a0fc094465fc2377a7a59bdef4df809c85e71e57bfc86c5cb7aefb08d30cd3f3603cc36eeba31dbd0e2cc88983e21eb1ddf3c" },
                { "gd", "a4b7774d2d7280cd91a8c09ba6e002efa2da08c4bbe6d8fedf5841dfd52153a9a81caf8e4565268ddd131d4455b1cd0d1d8290f5d4071a7c536bae7e1c10d376" },
                { "gl", "46273efedfc82711205ea87bd1789337b4134ed536dd1f97cdcb534d9c0d777fb060c1bc65e76f7c50bde518eaa62b31e12b4090423050dd530a3a3f10e3a39f" },
                { "gn", "4da242a6fa6e1a34c4890502b55bd4572af2b3795b98fda2f68700ce28d4e1e7c04ed4bd183cfda0881dd8cb47a7c8ecf96574068417c47951757bbdcb28e442" },
                { "gu-IN", "36530a15b2fb973f57dc2cf586782fbac3db120438c88c168f8d06f85917245cfa02c284cb6b8f406b65e757dfd91139c8e61902fd392224f0dd71597194767c" },
                { "he", "39bd7ee22802246915990c1a681c388e93722d394aeb6ac8a32edca9833d7b729396b30355af19958548574fa4c68f563b67ccc579afe7df0b021464e920fedb" },
                { "hi-IN", "4352a9b4ea5730eca195f47bdb6d96bd996cb0eb025cf63f6456f4879ff423723743c3baa3149b89fba0ffd8d8578a569240ab970f4293583b42a67fca77378f" },
                { "hr", "0ab7f7c33e0ce46461f8d208c159d00220c1cc54b4d9763bde33aa0f6d18185cbaeaf57854aec0a64f1b3ceff0ec968a2a7e958b0a30817893e0da7ab3048e81" },
                { "hsb", "11bdf2d1b875f6f47745ea42408cad00851dd48083b1c91188ac074c5d4a777122d417539e9e0af9d9cc2a9ae1360a39dc5e43cdcfee32cf9c84596586e702c8" },
                { "hu", "ea2928d0a0c716334ff44f6fc1967644e8a0cc77f5e290728b3d387e0bc2534a0b258f716368416a5fd272c75a28fe74caf132a9d96fa1a0c2fadf75e0f796aa" },
                { "hy-AM", "9d3f9cc712c9feabf33dbc091e81516a0f0b836658b9c6ba42b0ebac99b231a90f7891ba7d245be537559603ea3d9fa612e06a9a47c2d2ad9417d69ce62867ca" },
                { "ia", "75c268c5261feef992ba402a24ea7cf08a4c6aa92ebeecbcacd042f93905e8fdea2a7a14f8ca69b539b25c6c600d8e9f2b05cadaa9e9fc61b93f34ab3ec9cb8d" },
                { "id", "60c1ce1d41831123289642c800a0245cf06020db8df444a3b70a0c5b78010cc27ae376ec6b63210e4bf8cc2501ac31de3b2b2409a8bf5570cdc432b0c4410f2f" },
                { "is", "666d4c1f456df5b011de0ffa806e20943680fcccb19b8917d98dd05004319d053d3c7a2c1e3ad6bd5f63dfac3dc4e4281c8aefdc60ba0ff31ed5d60721cdf11f" },
                { "it", "d35976a8a831868b164828f94889182c261bd1649b537621c21f3856b783ec1bcd9089860d38bd2808e80460652b6da0fc5f3288634a6aa46e6fe5731dd29691" },
                { "ja", "1a2dcd2a5f14905bddd9cd13f59b0a33629cfdf43bcef0495821824e6f12a1cba96b7f7d59e1dbf66822e837e462a6ea45f976a377a8abbfe4e874de533ca7c8" },
                { "ka", "725711a76b25a2ec1875a7a8fe443df3543ec0a89ac71348d7b62b28cc92f74dc91f023b0ad30a5afe9c0dc9889d2a49d71d5f9fb9b362c9f21d9c4e95536bb6" },
                { "kab", "1b30294bd093d950f6487bdd61ec6bb015dca792ec1c3bb41556d5e5f29bb238df571fb6b88b33dbd77b563ef5cfe8cb0cbda7dcbd1cf07b719c4dd0c8ab6bd5" },
                { "kk", "fb578d73e068546ceb34249d348357753b4c306f76043b25c0a41986262dbdb8d93dc9d1e7fe63c786a28692a04726466fbea65f82bd8d50bb1ad290dcc1d495" },
                { "km", "d514c1c402a356865606618a6653186f8ed78d151b23e72df4e71064be6a96baab10f553791e35e335bd64eb2be16e81ffa7fecc86bd7cea6cf7ae56acefc41b" },
                { "kn", "baabea5e950cf80b00242b036f77893ad19051d51921510ab59a93e11c870119167e523eb638fb0e833c8950e2cf4e713f47e8b23e543c661cce25a76e81118d" },
                { "ko", "153ab39b9176de3b41a65fbe2d7053acafafebed5541d4072680b25092b89de81c5d3af0f868c331e1d78668407286df52ca554a4bbc8dc2c4b59d5647de1a26" },
                { "lij", "b7a89e79d388722b68d1d67bb3e985707c2c3ba3ebef82445624419b8dc14c1f772b01c97071ab52ea51eb5993d270367a834932fbb16224fb489aaec190423f" },
                { "lt", "2576d945119d9420a7f4a060034d8ba583280c144c74d1d0d27d1326cc1e86e12acf982213211e08fb5d1a6633f5c0ade994b870b1ee85e5141744fdec40a9ee" },
                { "lv", "2f6115bd7f596f66e4c6b8b13c4cb989c69c533713126a85552fc392730c1c048ebd0e275c33b10c553f1caa6047142d12275256a9c29b114bed72ceee028fd6" },
                { "mk", "cc5337cad71a2d4648380d2891e586353e91e2a08359837ad16bf759e62a4f04aeb60e6cf9ac1851041c0deb7146557ae4775295002f0ade4ca16ce58299c196" },
                { "mr", "88c8d0ab6c82dc59dfc896d04c5d4f7ff51053899990db316be25d4c4deafd4e5f0630926557817ca1ac571c139bb5fe0d58204c4250c450fa4598a82eecb191" },
                { "ms", "3fea5931a383d4c8b8e4dc2b934f8ba7ce33c125ec2b1957fc305e0d82129136a220e999577851f2ded13d2605efc41c74a4b26a7fe858ac0935945788870f0f" },
                { "my", "099c2fc74e3645381c175fd104b1148b953e54db56b14a2bd3da7a2cda0f491df78d3492af3e8fc0b2f1d474e044febd49c0d3c1052e2073e89cea659e28fbeb" },
                { "nb-NO", "0d7a8bf19ef9f8449236089a5ae88af6625396bec0b72b282529546bd7786d3c8027a1c1644b958cfbf07c42fcdd73813706a0a5c6a37438bbd6aa747a7ccc6d" },
                { "ne-NP", "9c9e25436b276ef8ec9f0ffa1a9305ddc2b07efe2c3d40a2580ed3dad920105ceb4c143df25edb2c29f26585206c034007954629c28639bbd1f6ce858c96068f" },
                { "nl", "80052fcac4b1e2a91ef0786f2babfb83dfb9472d809532a17ebdd9e4221d14ac84f6f06315c8ac13c9202ae4a34b1c94c62090a4b2c9049777046c63d6fa67c8" },
                { "nn-NO", "52ff97e3a8a28e0bd7b8775ec1c558447bd58936551bb346355690be9056254adf3197ee960e31f7c409455c127edec88026dba60effd34170c913bc047688b9" },
                { "oc", "c21cfe40277b77d5a54f96d1c936ee5e42a71568ca20e052cd76353eda4a336e8409f9ac5df0f5d363ef52f6fb3da938f5c84e5ecce935343efef470416364e9" },
                { "pa-IN", "be822be236a55b7335ea4cf51c64874eb526859b3a2e6084c497871aefe3ffeeeec681538e674f2e2f68419c0449ae186b2ed701cd69c27caf0a828584067e45" },
                { "pl", "5c116b15863212735105b6a729418beee9cb3e7948466972bf29dde62aa1ac468faa6be5599febea02feb524c6246effef77f9b5b9656227fecce8ea05671d51" },
                { "pt-BR", "9d5fd6f40cac3d0064f29c825d799deb1e239a8ce882dac9748caf17e3e8d351f249274a1c9c19683c0d92aec08ee1559e4eee2cc8c39251b444f2042fda8d9d" },
                { "pt-PT", "b53c8b2a4e38ea93c882d7da459921a7349f294019cd1ae466c4d389a749839fbdd1c3300269c24307f480e550044c209f142dc6f435d0276135d9d5c9b6d227" },
                { "rm", "c98f43454a529d556e357d51b962ba7041dff76453bbf5ed6adf4bbc42536447c5c22469abfb6d913f49b2908ce1b4314121bffa8975ff140f2a969d3a52a03c" },
                { "ro", "3410af616fb50aa371f5c2bb575750b5a1ded5d4584ea26d22249fb295f1391ca839a473d20ae9c40e9db7f41d5a5691139ee41ae3386c36ef494ac6efa51797" },
                { "ru", "0f13979421da752c9126e033f5b3bbd247e56b85e3662c7f7bca089b9a5e63b936d9f1ff360e99b760ab5aac4d7ef577a8b5c46ed9d8c21e99905c16bf8cf515" },
                { "sat", "c4eb708398a35e69e97d354e8903f2a955ee53c03e57fa6e786893aece4a5eb537264a81442097a337270da455d15e26320fb032173169013790cbdcd23209a2" },
                { "sc", "d80dbd4ca192787b5cf55effd4b80757a360334706cac444ce357c2ed9caeefabd2bea8589820d74c0026adbd3cef6cb29d36bfb9c4c77c457e276cdab2d49db" },
                { "sco", "fee9c67624e126f540c8d582009bc18f313cb36a9c6580f19685a392aac5ca0130c92a20ba2e701c3f19ff8d853f651f46371ed508a58b975a808a0ff059075a" },
                { "si", "bcf8e0beeab9b411ea6a134fc8e1ec20dee45294eb89d3bca1e9cc3bc9e77d52945fee8d993cd4dd61d5f4bcc5b7d2e9ed4d370a2eb1d9caaa0ede788c210499" },
                { "sk", "809fd54a7d59a085dfab008d5e23261ca5672703c482a47cc36e0c941b7299407857d189ad23470469ae0961b0beba084775f5ebb2ebc7da7e86fcdd16977046" },
                { "sl", "946941c751eb58484c1605923d6b18f8ff823cc40768bab2e0e47d591a27dba0fae0ca25553ecc41fb7d20156b6079f0db7147afed56564be4436bb8c2b93f19" },
                { "son", "5eb59ef34c91ee7d25e6fdd9b46a7e2f0ed4982c13411da88f6c639f709d476df4cc2392be595a930a1898a8e5a1f7c315113551015cd7b9159bf377aa50d2aa" },
                { "sq", "81f26771550f4e01f5b347ad041f8bfd275931bd541a4fd3b3f5754a17e6ab962bdbc0eaf7f565d1a3cb85463a90d71b3217998b655155508ce75a6fd9077262" },
                { "sr", "2d1d97188e9a8731266d3987edce09458b7548be8f235b09999b6605bc02dcb5c0a1624ac2c0c8ff9df04658b7d4ca473184558e4c50b9e7820511d1656e9803" },
                { "sv-SE", "46d77ae1c996022dd8e86f1b4500cc6a1f4632a7dc1702a0f90c280a9a74ce180a6ce2819b2cb30bbd12ad2a3847decff7989f8fa451d6cb95ad70666b191780" },
                { "szl", "cf2a79d1afc6646349c506111b99263576ae83de49f2c0f3011cb803889422dbee5b369ee2e29a59dee54b5e43da620f453c01fdff156d3d634fc6894b985ed8" },
                { "ta", "6f5ec78603976c4ea4886991325832f2220a31f73398c57843e9389a7d6b94795641c482178fcd80f40e0c38a68909030022eb35036e61a20fcf338bc7087225" },
                { "te", "7d6385bb4227b1e3a5570c138aa23ed16abfb773312b47cfbb52d8b4a67676936415526665229e4ffa011ad479a284b52fe2f1ec4b161dff2d41926fbb75bff8" },
                { "tg", "ac735c4e09cd1776116fb7414bd3e6396657aa8df5af2495dba6efd9262368742789098542d4c63ef01cc7b23719f8b90e9c0796b9a89162bc9d34f93ef9e7ff" },
                { "th", "1999829b1f00e45c30e2e57c71dddc8b3101d12c8bc19c80b96c9d28ddfd2630a69cadb927f82d4fd8cad361e84fb524d85bce2cd7fc0507e06aea0d9beed534" },
                { "tl", "0a795914b2ddc4ce0d814323225ae45658a0f3f264cbc56cdf23ddd1a00a689954767a490f7d232ab5f0e5d7c1908a5a3a562f7555b1ef39ddc884dd1747e2a1" },
                { "tr", "9d34a6becc9caefcc55ad242f9c1fdfb70b0274dd6767af8533ef4640542445c156c9d13417ffde0f1f19264ca76229a2eb4e83deff2dcd4b7214685a1e1e715" },
                { "trs", "715967d311efe2ea82ed50e1bd2611e5b8cb790ce3683ac1c7747b3a05953a53aecdc49758c54b7e5d7c64a4f3e3b303152879f39e429bef2a354353a84a159e" },
                { "uk", "dd53e9c63743c5932db77588e9122d124e4e19c94fc5e3fd5f7020de80f9a5115ac21357fb8931c9107fba752193ab23bf0e81ff1b9e766cb00eb4860e74133b" },
                { "ur", "416411f60c228d6e6b4b4b4aab52a83cdccc23f6e3e10a20b75cef735a46197ccc85ab3295acdd3353ec9892b44ae1dd7194108050f449ce9745d32d4f43f800" },
                { "uz", "610f4b02f2cb5c946c266d0ed3fdbbdc88fa24fd58aa9c70fc6161afc3d2bfd7690fbe23bfbf4492144eae5210ceed0a0484bada00d0fc717a942de1017a718b" },
                { "vi", "9df77169960ed36fb62e5efd542f080718cd7b993c4830d9dc1d767b5b15869dc5be97ad9c06dd84118d15ddd5f2d86c19cb5deb80df130d600823816319b2a7" },
                { "xh", "36595f9bb45c07feca9ea0cbb415b50fd53029399256f7d889dd9ca402a26d9c128270b02c0b285fb6592735cf55ca8ec558b9098ae431e5dc6f15317ba652d1" },
                { "zh-CN", "213a8ee36010d45a4d2a6d16c8ed199065ffd333cfc02310e21319c6aa41a5c7929b7f0bed6d9169b0b841366d77468ff936aa76df7f493d2c4445cd861b5883" },
                { "zh-TW", "059f04790a817ced0c4bfc367c4316476b2f41962fe1ade15280e6b3fa860bdab3224270f6d2c773ff952d58e30877c0648969ae9075fe9b78fb8ec6e8d76f05" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "805c3d87f986e380410e4e16c1e35e7304c4e7bbe284d2e573e35646bbe1683d7ef660518dee4af14dd8afd0aa9a4cee0240b2d35fff917928790a84cd8da578" },
                { "af", "4c4070a92190f59a669c54fe691b79b857f36642fccbe6c79796b9a866d1083edad2f9ac4520996800a2eca574a205a9ff482893a128751f369421dff4de3367" },
                { "an", "4b32b078cfcff599bc8f233ab81d1f16af011bb5d3f47e33cc893b6a35676a3e17df1f14b3421832b45b4cea37b1680736820eb378db1f1800262a3722e84582" },
                { "ar", "fa0062796263966d594a6b0f4f4302526d348d4762bb107cf17517852145b5f184a5fa45baf1a2aa26a778c33687c2d17dfd2b1641e9a160cdac274277fb6eec" },
                { "ast", "3359f468bd3ee1444fd1031d527700afebc2900fcd848420f17d01fee44b1ac608eb5f9a85f91843f2a17b3eaaf9120efce33ad27dc650795f2fa166dd81d392" },
                { "az", "0c58240597a8cec95aa3209c2c2d96610a306e0ba0c17f86da92d16c1fdc51cf43112368003b672df9cd7c3e0222d1c55dc31c7942f4c315a96da40690d5b493" },
                { "be", "c772f2809cc11f59631d724fb4c6986f48d8f3a564a053295403f04830e5ebd3462d0ccc4490b7f95781e502a589e51142160d9b61edded549e3a8590ed3e285" },
                { "bg", "4e88d1af9a147a74bcbc61136fb0fe750d4f42d617b0505f169199232420cfdf25f61f9e435a8b501cbe5274cc7e8169dabc7257feae38e184cbb24624cbe648" },
                { "bn", "7a10b24528df22ac350f03b7f7c4622e2aaf7c1f47791e34d40d562b3b665c47e242f7ea502ab6d685bc814e9980afded19c9c013d9e1ec85dc651b90c69d4cb" },
                { "br", "b85354d868fdd92b9183f01bbb72f73621a454e3758b2a62b3bafd11b97c009cff6f804c1f2b657ff0c308d2539465e3a20f0c6d0483bf703d1896dc6eb04773" },
                { "bs", "d153b05bb54c705d491ed0b5b6ebc1f637e482183069e5904bd8f5480111d9f28bff6b655cf051d6e8dc7f5caf46f2ac4e9325b78d02cd9dba8c6d23599f8e41" },
                { "ca", "35ba097b8d5052d3cece2d50208832bc2bc3de506d275bf3a595e3ef41b43707d458add12f44272853f3fa8234f5c9be3b4e4e2563d239505a07819fac8c2786" },
                { "cak", "bfbe8c016b16948841206c09327724432b2249b7c8d0585163493a302ea90d54478f707af1da4f8e465cc0fb3cd2c1d0f1b5c0df9eb0ac247f5e0284e25c67d5" },
                { "cs", "bb4034853f7946ec3e074e4d1b3c86b5605dfd48f320e5efbc8d715828f8a58b1d1d67a18b5d52b5661e59921a298d44c794b1b41a2185ddf52ebacaa287b396" },
                { "cy", "398cee31215a214e0841aea117de8116c63e803f361bf72ed6456841d548dbdbd56dda3023e9c034341d7cced71dc03863da50d1c3909f1ee115535733601a01" },
                { "da", "ba106826822515ad0ed78c1c9368538bf32388e5ff825c4af229b6c2bf456c5bebef66293dafe77c7c4b11190984bae8bab72e8d34d795368b7223aebdf7d5f9" },
                { "de", "863ab286e4b632f34445e15ca782262482a20799033b91c0be087de08954ea3d8d537c1882489dafe8c5cf0610650d8354bffdbb46024ce8fc760419e97f6310" },
                { "dsb", "e4cc9db3eed1d6801f3548013064a5028579eeb48fef3187b06b9ec5fa03de40fb1c7324376ee68dec9d27d72b789e2120767db7b1867f41209e87a2f0a3ba80" },
                { "el", "b754d8b029c4b1a2dc47c84d6443bdbd622cdfa8833763c037f7759a1c7f4d9b90c53326298c8d7c8b01780847d25985ee70a43af1901ef6eb1b9cf166b5c1ae" },
                { "en-CA", "ad2638da644448b127ad22c566738f5ce6f2478d2a2b6c315ef7b3486b69d7adffe1adeb29b979d17ea91aea32d3fbacdbdaafbb2e181737e0d386c76de85e98" },
                { "en-GB", "e7c29a65b4409a03156532fa84b692c74165b656fffe73217271726f178cb7c3a1b232bb18a8a43ecf88ba49590b7b20723e9761234fde52d64755ea0e704e84" },
                { "en-US", "f1da90c1c1156fa5dd453332d6b6f2692fdf22965b236fee437240fb26cc4acb83337400e4db12090b6a2d2c15babb35187ee3a26d67619aa5785a30a34b8f66" },
                { "eo", "82e95767b2c7cba4fbfefa5f7de57095a578f25f83cb214742fdebbef1e4d2d2e6c218ed4d979d7107f7343fd9e45b255fded6f94c78d6fec96ae1a44e9892c8" },
                { "es-AR", "dca78a0319a410c233e1202477f4a67b9d3f63a3108d6a2f40cb533491f0d03dbfe0530a4cfef803ea33bfb4e3d08aa7f6e22e92b8c8cf7b9da532c4a29a2179" },
                { "es-CL", "b29c3607ec33ad9d420dfc47ad56c977765fc8ecfe500955558a407dcd84c7f77133a76b51acd0cf898e185c2de8aebb2eb97b2e6c767d1602e235a393b0e9c4" },
                { "es-ES", "01848c90cfdc90d737317ca391df1949fbdd17179ff0e587208d43a2c4ac4f27616446038b37fb0c0be561b0584c4bb9339f58546589e44d6036369d910477d8" },
                { "es-MX", "69d8ded3b6d0eb26f642da647b260cac29bd5dce7afe4d9a0888d2bdd5da53850ba68773fdc303c70e3c52fbea9d88fb3017320b01cbee41e1fd78fd7f153a59" },
                { "et", "3749f4ac1fae6c13929a3f568f4f5e52c682d91ed0166a82e7bb94b4747835e1594c14c58de583c726b2a7b0d2f92d43ee2559a793c46a56555553ec6a7856e0" },
                { "eu", "246e63fdf69c5db75ad04a4da49abe45f8c05b03747695ed3e5ec37811615cb3d79de8032366a7b948dc2669250ea46cb5078aa7bc8dcae670e7bf5252fa949b" },
                { "fa", "e8c5e56ca2734ce7bed49509db90dd7130dbfc8a0798b50a8f9b43752238b3ecaf5c261dcfb0c76bda238a9a85000d1ed5e5f8cd535de659c5e7ef11637a9dc0" },
                { "ff", "e545afd8d485b517ecc35ad6566c6ffb708156e78e8b260d8a28a8904db656fd08535cf550696f517efff361891e40406a6c390e39f3898e5801a6d7a3a315bb" },
                { "fi", "5f097ea6862b85f5f03a8c2be6ee2d28ca7bed972adfa6896462128923d1ab2ee06fbc415647831fe4bf105b23ca62edbc8e55c66cbbcfff166d070f4f9d9826" },
                { "fr", "35de66dfd8c3d259a7d66d5a46f244cecda30fa4a62a423906c2d839d86bdea706c34ab5044b41b0212dbf33547df520b71d61c627961ae015e16231fd325700" },
                { "fur", "e1f03a1b59a0216eb254b6d41b56a44ff981ef84bef143aa702665c9e5d4676a5104de6cb15fe863e043f8810ed559f708c1f997f2540ca7c0466da7bf43cdf7" },
                { "fy-NL", "a868a3ed4e9e15d42aa86c34d2799c9be9b1c18d3af88ff6208e82a391970f36550c535bb82d9ac39b2d0efb0604c02b05ea1b237a9e4b2adc7ba20dc79dd406" },
                { "ga-IE", "88372874beedfe359f179547f82dd5887b5ecc49474e27f663d29796be276679cf18f8330ad77a17868faa0d758d54fba5ea109fa4e4973ff0ef718f068626ba" },
                { "gd", "f46579cf4907338bce33f26469a150512413ce203b23ea344684e8caa7e175023e9089e8d4dbad33f8ca4c0dfdbb07762b1d0e4d5a825c472f1f2510ca75aac6" },
                { "gl", "e338b2777030308b23c96dcf024b7f77d4cad8aaf465aa5c861dd7e866e94efd4b10709ed132bd1d5766ad4a8360984a9a078d2bc421657292d6ed0ce677940a" },
                { "gn", "c34f79b5f57bb700fdb1ada36b3549d5a1e928af2f4c0ad8a6e7b9e8236057f79eb1a39fb86f706510d3a400d800de081e91ef80a036443febe1fdb0a98e5af0" },
                { "gu-IN", "61b5af398e11ddbfce915c80f444d5e78b4097af64df2c2a1da0f487b7198482c8ff507ea978238b7851d07b53cdace6e3e601590e48c5b1f663d756fc80ed06" },
                { "he", "a8ebb8a6ddeae92cd3637781784ba3e852662711cb3dee7c751b074376c1a369e6717a4e3eda24503dd193905712a93cf38adedf4f81288b6e4db07412c51f06" },
                { "hi-IN", "f64e7903700a6689405f6ff09af71d149a052963b170604fdbbb0f899a68a13bdc3bf88437d72c2a667e88e85fcab920d4ea15a2a0acf4c08fde564f8dbd6b3e" },
                { "hr", "1777139e0db4dbe57a2efca7ecf9abf1c330d0aeda72eb264ae75935d9230120877c4a64f003479821c112357bd0256b210d599f90133c72971db5a2b94d6013" },
                { "hsb", "0d3be6687371cd00c0f395a558abd1ff2ca06bc76c44ca0038b6387393270e73859e11ec3d51096dc7393d1dd2516a636d97045acfa3792f2018889dfe8f33d6" },
                { "hu", "8eb234d0cd30791b3a4ba606c14a4a72848c2b6be6d1b8dbcf33af98d6516ff1ceb094806e15f588c173a0717ff5b5e0aded331a8093146ee516ab7e8d1637c2" },
                { "hy-AM", "e256d2710bb8cdebee5107ce689a1123dbe18e7b29b987843a51e9f0534661e648158e7aa26dcecc584354d373bc44f7e9e7b4d84b58087d73572358e81832f2" },
                { "ia", "31afd44deda6241513aaf52e4395f4eae71bfe274308f49c421b4ac9bc4e1c0e1209da0bd5e6dc65d2031c72c15079eaa542e9b6b0732d9f7752deac1b272d25" },
                { "id", "4cf9207493450fa8ee475abca7f0719f1e711d8b09272f5a7c5993f5e2079d70ba3430b2f03881177d6e37b0a9c718f7360a3714e92a74299638fcc03b687680" },
                { "is", "94954bc22bc8279e3d8e81ba99f07ef827898049a831372193c76c1786b1e0520a45d8dfbf52ac69067889bb7a06af70bfa8dedd58ef6f1acbf635783f2e1791" },
                { "it", "351783aedacfcf961b8e7334e131dc09673a858f5e68e93069283464fb980a7cdcb7924184096b106f42d7eaf120a227011f6274f1fd6d952084947a18269650" },
                { "ja", "660f962ec39fcb6a27fc24d786bb28f248071f9ec080398ec828f84ee4fad89b2e96e3b082f00ae431183a6c45226f22f44918f54d13807a82e10818804b3ec2" },
                { "ka", "7117fc130b47409387beff1f7487f31c2397d98341f6d3919c3544ecfe77542446d6631f3fec458973567bb7ad2902ecc4ce137b9f93de9fd9ef0aca6059ffea" },
                { "kab", "af28b325bb77c886f3df264e05d3b0de03ffc0846d0574726cfdeefa676221e8b9a99219201658210ece1fbd341c08c9f240919f0f0ff0dfb5a45e51ec0aa8b6" },
                { "kk", "d243e7dd5f387e1b59cd8818d9347ca51af69551a52a1aa001d806b3dd97a9e5f73fa8ad25f7e7d1b6b688f9e65ff2da544d5baefbf5d4c5393a5d37f00cd1c8" },
                { "km", "2a2ee1e5782d7c7576ee76a814ea244911e38d38f5b5d3590e2c8243f7fabd8f27b37f22a93cce612a9408e2763b5b5679fd50b1c00a2cb62c75ae8955d14dd0" },
                { "kn", "42477ce5596777b82b3945e3ef6263011d8c9272e96d19570cce66d5e9656d1c9c33dc328a6b6d786bab6a689d9aae18c394f9cebbf24ea80f8da922d08fd1d7" },
                { "ko", "827bc705201035a6ac90deddaf7bed24be2d84f947b411e3097ff371d3cde0c3724580738dcc12b5cce5c0aa6d05d258aba8b934668363b36d0e31da242ee0e3" },
                { "lij", "20740dff879843acab475dc718849544372fad76bf5cd094cc627beb5383c82fe5d905696e18efbe6f6786aca0d21bce7acd2224f1df355e15f470ef8382d7e2" },
                { "lt", "d8f793ba06c4ab1b45e8ac150f821529443affb8590beee7054f5e05e20bc2b8b9d427b13d6e6b17718928df1b417711e01388123d900020cdf56eeea629cf2d" },
                { "lv", "1613bf0bb579259bddf8ba5cb59c436abee1d11e855d55e6ee31be78f2358abd4de0b859bf5448633a04288198444bf696f5dfbaab73a453d3c08c4a1eb0196f" },
                { "mk", "5f74b7d138759e757c9f35c527de2cd09fd628b572de26f94d5679389c39ec4b8915379d8ddf62bc3f2ec45175098a261c6271bc87e3fc4eeccaed0c1b123296" },
                { "mr", "81d713a4c2d6d1267166c2c1c5c69b1a3648f89c0fca0dfbbc5e2127185af1871511f190d57c92290faf7725b587a35fbafb8969b9124d9905042a96f89318da" },
                { "ms", "84e340f3e87d6a22b35f27578b152113d1eae9619b9834ae3ab70937aac80b055d1d944718e33f74603a00edf2e073a469d586b76b2cee875e72539f05a547df" },
                { "my", "b3d518eee15ab92bca14a28272f9b01ca90c36ed20fc6940b840090c1deefa176484898aad071da874a5a6a5bc2f15669761a36414147a558328800462220c01" },
                { "nb-NO", "c9a011c8d807cbe6139e9e89a07775d7e49d871e90777d767569dab3cedafda689d570fe7a9a8c1688780fbe40c32383feab5245b659279e578a2f311dd309ea" },
                { "ne-NP", "751da37627a462d0c00dbc5152731edf4cd404b97e079f8f8e18f6944bcb280fceb49e941fd135441ed710698f859c2196767e1e910d788746ecacc970d2c8a4" },
                { "nl", "300440f645cf63f06bf984add464d78bbce35e057bec6dcf35bba368efe4dcacf9b8cc35d3b7ff65496cbdaf3e613c448a0132f72d6ac11338fa087da6b71051" },
                { "nn-NO", "34354cd2ba79912f2946ef637960f75544745f5b1a3f47b8c7647a2a36de80a805d4fafd8db5834dc15bc929be8af5d03227bb441c973e72bb41260197e8e04b" },
                { "oc", "d502d423a4632fa4ad65690390b51e8f252956e8b4bc0b7f8c8b59f9658011865669c53626a3b7f708b5f9be506b7284dab1406e5ed150b7550ad25650438406" },
                { "pa-IN", "b20796f96ca86c35ee2476bbc5437924a08eb9d70f943acf7cbf6792b77d28aae5d142d17a90163a38198df615362b9862f13ecd3d24a463e1071032974a647b" },
                { "pl", "8526dc9ade87c08937698aaafccfb3e3ac6f2b84e861d93b9ba32a71248b798392a48868707908ac84cac3e813ee5f46264b5ac636fad31060239244fbe1fc1b" },
                { "pt-BR", "cb4462af140a41ce22c723a65b80dbda39a7709199667725751da5b9fa767800858ef0ef43c9e62137b129f23916099e030e7ba3bf4698e75b16beaa95983032" },
                { "pt-PT", "f28f5b05921c1d32921cc03ddd05b0270e8e0213b460fd5b1b668e26152276980a85faa18b887bc3f4e0376d6cae73ee9d9878601380f35e838c2d8fb1ff2efc" },
                { "rm", "de8d5f042abe6e760144e179fd2b4472b75f4e18ce03105ce9829171640ce056241b7fcb2a1888becd9a1d5b3f0e39997441c363dd75eaf19fa93e472900c366" },
                { "ro", "d14f8a4654265b110450640300be1d00742a376576e500954d2d4526bdfe8b1f7881777160c9c68ff7c227543ecf177b4985c2f5fdd220997d321ad5021a2f0e" },
                { "ru", "fc10d321c63c95cc81e6d14abbc6b9dc4e7e2ab2a9098558ef0c45bf0fd1bc3440c4814cdfa546256b90c8605e2e0172f3821d09ee639e146c2363acae93f97e" },
                { "sat", "b93acd2ca234847e9d3851ba1093c3ded08c6440732c54931f2604291645fbab2041d01793e9336201f2f1a129c9c03b13f8562bc80858c305a84e5cc3f35e05" },
                { "sc", "9d6277ec1639f5f3281b9856af70ebcba0f882708d7c99a64435a3ce73423a2b28dca50beb7fe26ae8288272bc4ef18c6eb49fc7a332ddd0dbec12809a40b555" },
                { "sco", "634df4882c74ea94c3a57ed6214f8b26716294e03fad3d2dfe3345a602c7a3e9d4af529eb152824908213095ddd3ef6025ef0986c2c267e5ea8ec23feb88d67c" },
                { "si", "70568cf00289d67b31596eff7a70509c782cc041a22bacae712af269aef629db14f8ea8f926daf5efbc120a3ee58e05c5226eb596ca6893ff67c95cea9ccfae4" },
                { "sk", "e516ec0cd59370c6249cee5631034e1233f30e2603b16c9875a4e423836b5b8156a177fbf055f066f0ba8972ff98d10c448d8b4c23c90d0dfe0664ccad957c2b" },
                { "sl", "a55449376dc659c0a64e113fb145003fa3b1d8fba66a2e03c77b678385a369b5f4dcb93dc9c6321617df8d9f8e43e6818126d4011b636b867549ef213f4a2ee1" },
                { "son", "680145b38999092312dafee4106de2276ac7109037f3113018c6e543a6cea377b52c2a4e57d4abec58ba66e28602c7c069e826a159b7fd5b082732de5336fa00" },
                { "sq", "ad70e6cb50236fb28fefc3e33f02e3a8e7013b4bb7d5245a1307a1889feb1b8e92214cf63d8d433aa67017292d6417b3b5d2cdf7ea6efcfbf7c4f9acbcd746fe" },
                { "sr", "efce790f60369faa837ce945955320fcced4fb5b2b053f970bc472681956b423c419c8b78aedd6cf3611a2c3879dccd076df2659be0f9b6f81abd1e8b658e1f1" },
                { "sv-SE", "765198fd21254db7c94a77d4d42e217adecf56c4d10d081593f5550c80812fa6537858c27b444512f5bd58bfc298c350b28fed46f2beb61b333745b65213b61c" },
                { "szl", "6bc33984805b6160a48eff4f1f4a81f09fd0b538f0d39eee1c134d0f10d46136a8c27b09e6f8bf60b1a246381ccde53dc217b6006871562361e0b00de8019f31" },
                { "ta", "d718abedeeb36b441f8ea008ecf6ec0b90f06b603c993a35d70f065d744c96fff3d92e02c15daec94078e15eea0994e21300fcf5dfc0acebb12968f0042e761e" },
                { "te", "edfd10695e34c9c035b199a6cf1f96af02a3567e56c0d0e2cd268f23877fd673e18841e4df80736767e8b15e36adacb543146773b4925d82ad7b521e979e9703" },
                { "tg", "b980e2cc2e5389f6cc5ad49ffed6630427ef1ecc2e904eea93d9888175b433547745931c61ecc22e9b00e176f644518ccdc20ab644ad179f0d273f82a70b2e0b" },
                { "th", "1bd585806b5ebf307f4340cf4260af72d1d5ba1cdce596545527cfb17ebb257282398790cee68a970e8af8f51472009af8ba9a9a14a19679fd29d0727ab00699" },
                { "tl", "0ae3e1432a6bdaaeda038879d93d840b2f5ff682a6b73285c4973005c94d0cf9da5f958a77b453b442360ab9cb8409132b0463fce2dbfcb37539d25ae4415f67" },
                { "tr", "d3a40ada19244affb6e307e6c37c00d9a9f7ea59ad1f5a709f69373b269f9b1a1477613a0e1225f4d716f0ad7544419dac429fe292e87c79d7bfb1437a0e4fa5" },
                { "trs", "034d085c52922889cd260d6819b27b835fc67da58f920e7b84e63de9c60a8d9098cc8edac85569e9eafc8da8106cc03c474c6368be7f0223f3f3a1cbc7169568" },
                { "uk", "5b6c9295f854512fa0b6f4c18a2b00441a2011a37849247892c25b964db438e4cbe1f7a32fec9e62bf0c139327e076a9b8c084a38298dc9f5b3989100e6d9ad6" },
                { "ur", "9e31133be9a268645d4e556fa3500b985b985fea17eb19ef212eae340b6825b95247be6c408606ab197a71cdeb173d410a6457ece1caaf30826d48b60734f304" },
                { "uz", "9814d1687b181a1135fbffce2621d19cac73126c99eaca71fdf3a1707072b86494b416919ddc54d6cd96e8943035133a797b0d0eac93a56db6a253d2578eafe8" },
                { "vi", "1642cb1059746e1e3c05dfdde0b35726affe111f64944fec115b8df9c2d54705a6dd736d37cd3ad4edb2429a3eea28ea50cadac9a64a792044115ca03a4ac4fc" },
                { "xh", "86a50da6a1ee81dc3cb33f278d5c4f069fa7d9acd8d1b09f79a8a7aebf1b2bf5cbbf3bbd79b5843b497893c8d9fbf9d832db8dd291c77dbdc97ec394130fc77c" },
                { "zh-CN", "1089ee0a652bb388689c327c20d7de492ec42c70bfc45fb91ab39ed99c6ef8c6ccdb4c622f36109051712aed928329f82a2d839efb4b38bcbbc88ea46df7b0c2" },
                { "zh-TW", "e637c7fe1a71ca009ad9fc69bf66eaec6fb865af458ee751c0e65851fe90d7988c85bda0e2df3f745184700ad5346bc8549528ec590f4fd3a0e221020e1bf982" }
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
